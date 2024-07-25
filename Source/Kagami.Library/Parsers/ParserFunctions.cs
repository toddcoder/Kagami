using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Expressions;
using Kagami.Library.Parsers.Statements;
using Kagami.Library.Runtime;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static System.Int32;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;
using Array = System.Array;
using Parameter = Kagami.Library.Invokables.Parameter;
using Return = Kagami.Library.Nodes.Statements.Return;

namespace Kagami.Library.Parsers;

public static class ParserFunctions
{
   public const string REGEX_FIELD = "['A-Za-z_'] ['A-Za-z_0-9']*";
   public const string REGEX_INVOKABLE = "['A-Za-z_'] ['A-Za-z_0-9']*";
   public const string REGEX_CLASS = "['A-Z'] ['A-Za-z_0-9']*";
   public const string REGEX_CLASS_GETTING = REGEX_CLASS + "('.' " + REGEX_CLASS + ")?";
   public const string REGEX_ASSIGN_OPS = "'+' | '-' | '*' | '////' | '//' | '^'";
   public const string REGEX_FUNCTION_NAME = "((" + REGEX_INVOKABLE + ") | (['~`!@#$%^*+=|\\;<>//?-']+) | '[]') '='?";
   public const string REGEX_EOL = "/r /n | /r | /n";
   public const string REGEX_ANTICIPATE_END = "(> (" + REGEX_EOL + ") | $)";
   public const string REGEX_OPERATORS = "['-+*//\\%<=>!.~|?#@&^,;.:']";
   public const string REGEX_ITERATOR_FUNCTIONS = "'sort' | 'foldl' | 'foldr' | 'reducel' | 'reducer' | " +
      "'count' | 'map' | 'flatMap' | 'bind' | 'if' | 'ifNot' | 'index' | 'indexes' | 'min' | 'max' | 'first' | " +
      "'last' | 'split' | 'one' | 'none' | 'any' | 'all' | 'span' | 'groupBy' | 'for' | 'while' | 'until' | 'z' | 'zip' | 'x' | 'cross' | 'fold' | " +
      " 'seq' | 'takeWhile' | 'takeUntil' | 'skipWhile' | 'skipUntil' | '!' | '?' | '*' | '@' | '$' | '.' | ':'";
   public const string REGEX_LIST_LEFT = "⌈";
   public const string REGEX_LIST_RIGHT = "⌉";

   public static Optional<char> fromHex(string text)
   {
      var _char = $"0x{text}".FromHex();
      if (_char)
      {
         return _char.Map(i => (char)i).Optional();
      }
      else
      {
         return fail($"Didn't understand {text}");
      }
   }

   public static Result<char> fromBackslash(char original) => original switch
   {
      'n' => '\n',
      'r' => '\r',
      't' => '\t',
      _ => fail($"Didn't understand {original}")
   };

   public static Optional<Expression> getExpression(ParseState state, Bits32<ExpressionFlags> flags)
   {
      var expressionParser = new ExpressionParser(flags);
      return expressionParser.Scan(state).Map(_ => expressionParser.Expression);
   }

   public static Optional<Expression> getExpression(ParseState state, string pattern, Bits32<ExpressionFlags> flags,
      params Color[] colors)
   {
      return
         from expression in getExpression(state, flags)
         from scanned in state.Scan(pattern, colors)
         select expression;
   }

   public static Optional<Expression> getCompoundComparisands(ParseState state, string fieldName)
   {
      var flags = ExpressionFlags.Comparisand | ExpressionFlags.OmitAnd | ExpressionFlags.OmitIf;
      var builder = new ExpressionBuilder(flags);

      var _comparisand = getExpression(state, flags);
      if (_comparisand is (true, var comparisand))
      {
         builder.Add(new FieldSymbol(fieldName));
         builder.Add(comparisand);
         builder.Add(new MatchSymbol());
         var _scanned = state.Scan("^ /(|s|) /'&'", Color.Whitespace, Color.OpenParenthesis);
         if (_scanned)
         {
            return getCompoundComparisands(state, fieldName).Map(nextExpression =>
            {
               builder.Add(new AndSymbol(nextExpression));
               return builder.ToExpression().Optional();
            });
         }
         else if (_scanned.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            return builder.ToExpression().Optional();
         }
      }
      else if (_comparisand.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return nil;
      }
   }

   public static Optional<Container> getInternalList(ParseState state)
   {
      var builder = new ExpressionBuilder(ExpressionFlags.Standard);
      var constantsParser = new ConstantsParser(builder);

      while (state.More)
      {
         var _responding = constantsParser.Scan(state);
         if (_responding)
         {
            if (state.Scan("^ /(|s|) /','", Color.Whitespace, Color.Operator))
            {
            }
            else if (_responding.Exception is (true, var exception))
            {
               return exception;
            }
            else
            {
               break;
            }
         }
         else if (_responding.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            break;
         }
      }

      var _symbols = builder.ToExpression().Map(expression => expression.Symbols);
      if (_symbols is (true, var symbols))
      {
         List<IObject> list = [];
         foreach (var symbol in symbols)
         {
            if (symbol is IConstant c)
            {
               list.Add(c.Object);
            }
            else
            {
               return fail($"Expected constant, found {symbol}");
            }
         }

         return new Container(list);
      }
      else
      {
         return _symbols.Exception;
      }
   }

   public static Optional<Operation> matchOperator(string source) => source switch
   {
      "" => nil,
      "+" => new Add(),
      "-" => new Subtract(),
      "*" => new Multiply(),
      "/" => new FloatDivide(),
      "//" => new IntDivide(),
      "^" => new Raise(),
      _ => fail($"Didn't recognize operator {source}")
   };

   public static Optional<Block> getBlock(ParseState state, Maybe<TypeConstraint> _typeConstraint)
   {
      var _advanced = state.Advance();
      if (_advanced)
      {
         var statementsParser = new StatementsParser();
         state.PushStatements();
         while (state.More)
         {
            var _scanned = statementsParser.Scan(state);
            if (_scanned.Exception is (true, var exception))
            {
               return exception;
            }
            else
            {
               break;
            }
         }

         return state.PopStatements().Map(statements =>
         {
            state.Regress();
            return new Block(statements, _typeConstraint);
         }).Optional();
      }
      else if (_advanced.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return badIndentation();
      }
   }

   public static Optional<Block> getBlock(ParseState state) => getBlock(state, nil);

   public static Optional<Block> getSingleLine(ParseState state, Maybe<TypeConstraint> _typeConstraint,
      bool returnExpression = true)
   {
      var statementsParser = new StatementsParser(true) { ReturnExpression = returnExpression, TypeConstraint = _typeConstraint };
      state.PushStatements();
      var _scanned = statementsParser.Scan(state);
      if (_scanned)
      {
         var _statements = state.PopStatements();
         if (_statements is (true, var statements))
         {
            return new Block(statements, _typeConstraint);
         }
         else
         {
            return _statements.Exception;
         }
      }
      else if (_scanned.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         state.PopStatements();
         return nil;
      }
   }

   public static Optional<Block> getSingleLine(ParseState state, bool returnExpression = true)
   {
      return getSingleLine(state, nil, returnExpression);
   }

   public static Optional<Symbol> getValue(ParseState state, Bits32<ExpressionFlags> flags)
   {
      var builder = new ExpressionBuilder(flags);
      var parser = new ValuesParser(builder);

      return parser.Scan(state).Map(_ => builder.Ordered.ToArray()[0]);
   }

   public static Optional<Parameters> getParameters(ParseState state)
   {
      var _scanned = state.Scan("^ /[')]']", Color.CloseParenthesis);
      if (_scanned)
      {
         return new Parameters();
      }
      else if (_scanned.Exception is (true, var exception))
      {
         return exception;
      }

      List<Parameter> parameters = [];
      var defaultRequired = false;
      var continuing = true;

      while (state.More && continuing)
      {
         var _parameter = getParameter(state, defaultRequired);
         if (_parameter)
         {
            if (_parameter.Value.DefaultValue)
            {
               defaultRequired = true;
            }

            parameters.Add(_parameter);
            if (_parameter.Value.Variadic)
            {
               continuing = false;
            }
         }
         else
         {
            return _parameter.AnyException;
         }

         var _next = state.Scan("^ /(/s*) /[',)']", Color.Whitespace, Color.CloseParenthesis);
         if (_next)
         {
            if (_next.Value.EndsWith(")"))
            {
               return new Parameters(parameters.ToArray());
            }
         }
         else
         {
            return _next.AnyException;
         }

         if (!continuing)
         {
            return fail("There can be no parameters after a variadic parameter");
         }
      }

      return openParameters();
   }

   public static Optional<Expression[]> getArguments(ParseState state, Bits32<ExpressionFlags> flags)
   {
      var _scanned = state.Scan("^ /[')]}']", Color.CloseParenthesis);
      if (_scanned)
      {
         return Array.Empty<Expression>();
      }
      else if (_scanned.AnyException)
      {
         return _scanned.AnyException;
      }

      var arguments = new List<Expression>();
      var scanning = true;

      while (state.More && scanning)
      {
         var _expression = getExpression(state, flags | ExpressionFlags.OmitComma | ExpressionFlags.InArgument);
         if (_expression)
         {
            arguments.Add(_expression);
            var _next = state.Scan("^ /(/s*) /[',)]}']", Color.Whitespace, Color.CloseParenthesis);
            if (_next)
            {
               var next = _next.Value;
               if (next.EndsWith(")") || next.EndsWith("]") || next.EndsWith("}"))
               {
                  return arguments.ToArray();
               }
            }
            else
            {
               return _next.AnyException;
            }
         }
         else if (_expression.AnyException)
         {
            return _expression.AnyException;
         }
         else
         {
            scanning = false;
         }
      }

      return openArguments();
   }

   public static Optional<(Expression[], Maybe<LambdaSymbol>)> getArgumentsPlusLambda(ParseState state,
      Bits32<ExpressionFlags> flags)
   {
      return
         from arguments in getArguments(state, flags | ExpressionFlags.OmitColon)
         from lambda in getPossibleLambda(state, flags)
         select (arguments, lambda);
   }

   public static Responding<IObject> getComparisand(ParseState state)
   {
      var _expression = getExpression(state, ExpressionFlags.Comparisand | ExpressionFlags.OmitComma);
      if (_expression)
      {
         if (_expression.Value.Symbols[0] is IConstant constant)
         {
            return constant.Object.Response();
         }
         else
         {
            return constantRequired(_expression);
         }
      }
      else
      {
         return _expression.AnyException;
      }
   }

   public static Optional<IObject[]> getComparisandList(ParseState state)
   {
      var _scanned = state.Scan("^ /[')]']", Color.CloseParenthesis);
      if (_scanned)
      {
         return Array.Empty<IObject>();
      }
      else if (_scanned.AnyException)
      {
         return _scanned.Exception;
      }

      var arguments = new List<IObject>();
      var scanning = true;

      while (state.More && scanning)
      {
         var _comparisand = getComparisand(state);
         if (_comparisand)
         {
            arguments.Add(_comparisand.Value);
            var _next = state.Scan("^ /(/s*) /[',)]']", Color.Whitespace, Color.CloseParenthesis);
            if (_next)
            {
               if (_next.Value.EndsWith(")") || _next.Value.EndsWith("]"))
               {
                  return arguments.ToArray();
               }
            }
            else
            {
               return _next.AnyException;
            }
         }
         else if (_comparisand.AnyException)
         {
            return _comparisand.Exception;
         }
         else
         {
            scanning = false;
         }
      }

      return openArguments();
   }

   private static Optional<bool> parseReference(ParseState state)
   {
      return state.Scan("^ /(/s* 'ref' /s+)?", Color.Keyword).Map(s => s.IsNotEmpty());
   }

   private static Optional<bool> parseMutable(ParseState state)
   {
      return state.Scan("^ /(/s* 'var' /s+)?", Color.Keyword).Map(s => s.IsNotEmpty());
   }

   private static Optional<string> parseLabel(ParseState state)
   {
      return state.Scan($"^ (/(/s*) /({REGEX_FIELD}) /':')?", Color.Whitespace, Color.Label, Color.Structure)
         .Map(s => s.KeepUntil(":").Trim());
   }

   private static Optional<bool> parseCapturing(ParseState state)
   {
      return state.Scan("^ /(/s* '+')?", Color.Structure).Map(s => s.IsNotEmpty());
   }

   private static Optional<string> parseParameterName(ParseState state)
   {
      return state.Scan($"^ /(/s* {REGEX_FIELD}) /b", Color.Identifier).Map(s => s.Trim());
   }

   public static Optional<Maybe<TypeConstraint>> parseTypeConstraint(ParseState state)
   {
      var _className = state.Scan($"^ /(|s|) /({REGEX_CLASS}) -(> '(') /b", Color.Whitespace, Color.Class)
         .Map(cn => cn.TrimStart());
      if (_className)
      {
         var _baseClass = Module.Global.Class(_className);
         if (_baseClass)
         {
            return new TypeConstraint(new[] { _baseClass.Value }).Some();
         }
         else if (Module.Global.Forwarded(_className))
         {
            return new TypeConstraint(new[] { new ForwardedClass(_className) }).Some();
         }
         else
         {
            return classNotFound(_className);
         }
      }
      else if (_className.AnyException)
      {
         return _className.Exception;
      }
      else
      {
         var builder = new ExpressionBuilder(ExpressionFlags.Standard);
         var typeConstraintParser = new TypeConstraintParser(builder);
         var _scanned = typeConstraintParser.Scan(state);
         if (_scanned)
         {
            var typeConstraint = (TypeConstraint)((IConstant)builder.Ordered.ToArray()[0]).Object;
            return typeConstraint.Some();
         }
         else if (_scanned.AnyException)
         {
            return _scanned.Exception;
         }
         else
         {
            return nil;
         }
      }
   }

   private static Optional<bool> parseVaraidic(ParseState state)
   {
      var _scanned = state.Scan("^ /(|s|) /'...'", Color.Whitespace, Color.Structure);
      if (_scanned)
      {
         return true;
      }
      else if (_scanned.AnyException)
      {
         return _scanned.Exception;
      }
      else
      {
         return false;
      }
   }

   private static Optional<Maybe<IInvokable>> parseDefaultValue(ParseState state, bool defaultRequired)
   {
      var _scanned = state.Scan("^ /(/s* '=') -(> '=')", Color.Structure);
      if (_scanned)
      {
         return getExpression(state, ExpressionFlags.OmitComma).Map(e =>
         {
            var symbol = new InvokableExpressionSymbol(e);
            state.AddSymbol(symbol);
            return symbol.Invokable.Some();
         });
      }
      else if (_scanned.AnyException)
      {
         return _scanned.Exception;
      }
      else if (defaultRequired)
      {
         return fail("default required");
      }
      else
      {
         return nil;
      }
   }

   private static Optional<Parameter> getParameter(ParseState state, bool defaultRequired) =>
      from reference in parseReference(state)
      from mutable in parseMutable(state)
      from label in parseLabel(state)
      from capturing in parseCapturing(state)
      from name in parseParameterName(state)
      from typeConstraint in parseTypeConstraint(state)
      from variadic in parseVaraidic(state)
      from defaultValue in parseDefaultValue(state, defaultRequired)
      select new Parameter(mutable, label, name, defaultValue, typeConstraint, reference, capturing) { Variadic = variadic };

   public static Optional<Block> getAnyBlock(ParseState state)
   {
      var _response = parseTypeConstraint(state);
      if (_response)
      {
         var _typeConstraint = _response.Value;
         state.SetReturnType(_typeConstraint);
         var _scanned = state.Scan("^ /(|s|) /'=' /(|s|)", Color.Whitespace, Color.Structure, Color.Whitespace);
         if (_scanned)
         {
            return getSingleLine(state, _typeConstraint);
         }
         else
         {
            return getBlock(state, _typeConstraint);
         }
      }
      else if (_response.AnyException)
      {
         return _response.Exception;
      }
      else
      {
         return nil;
      }
   }

   public static Result<If> getIf(string parameterName, Symbol comparisand, Maybe<Expression> _and, Block block)
   {
      var builder = new ExpressionBuilder(ExpressionFlags.Standard);
      builder.Add(new FieldSymbol(parameterName));
      builder.Add(new SendMessageSymbol("match", new Expression(comparisand)));
      if (_and)
      {
         builder.Add(_and);
      }

      return builder.ToExpression().Map(expression => new If(expression, block));
   }

   public static Optional<LambdaSymbol> getPartialLambda(ParseState state)
   {
      if (!state.More)
      {
         return nil;
      }

      var unknownFieldCount = 0;
      var maxFieldCount = 0;
      var addOne = false;
      var builder = new ExpressionBuilder(ExpressionFlags.OmitComma);
      var unknownFieldParser = new UnknownFieldParser(builder);
      var valuesParser = new ValuesParser(builder);
      var postfixOperatorsParser = new PostfixOperatorsParser(builder);
      var infixParser = new InfixParser(builder);

      Responding<Unit> getLocalValue()
      {
         var _unit = valuesParser.Scan(state);
         if (_unit)
         {
            return _unit;
         }
         else if (_unit.AnyException)
         {
            return _unit;
         }

         _unit = unknownFieldParser.Scan(state);
         if (_unit)
         {
            maxFieldCount = unknownFieldParser.Index.MaxOf(maxFieldCount);
            addOne = true;
            return unit;
         }
         else if (_unit.AnyException)
         {
            return _unit.Exception;
         }

         builder.Add(new FieldSymbol($"__${unknownFieldCount++}"));
         return _unit;
      }

      Responding<Unit> getLocalTerm()
      {
         var _unit = getLocalValue();
         if (_unit.AnyException)
         {
            return _unit;
         }

         while (state.More)
         {
            _unit = postfixOperatorsParser.Scan(state);
            if (_unit.AnyException)
            {
               return _unit;
            }
            else
            {
               break;
            }
         }

         return unit;
      }

      state.BeginPrefixCode();
      state.BeginImplicitState();
      state.Scan("^ /(|s|) /'('", Color.Whitespace, Color.OpenParenthesis);

      try
      {
         while (state.More)
         {
            if (state.CurrentSource.StartsWith(")"))
            {
               break;
            }

            var _unit = getLocalTerm();
            if (_unit.AnyException)
            {
               return _unit.Exception;
            }

            _unit = infixParser.Scan(state);
            if (_unit)
            {
               _unit = getLocalTerm();
               if (_unit.AnyException)
               {
                  return _unit.Exception;
               }
            }
            else if (_unit.AnyException)
            {
               return _unit.Exception;
            }
            else
            {
               break;
            }
         }

         var parameterCount = unknownFieldCount.MaxOf(maxFieldCount) + (addOne ? 1 : 0);
         var _scanned = state.Scan("^ /')'", Color.CloseParenthesis);
         if (_scanned)
         {
            return builder.ToExpression().Map(expression => new LambdaSymbol(parameterCount, expression)).Responding();
         }
         else
         {
            return _scanned.AnyException;
         }
      }
      finally
      {
         state.EndPrefixCode();
         state.EndImplicitState();
      }
   }

   public static Optional<IConstant> getConstant(ParseState state)
   {
      var builder = new ExpressionBuilder(ExpressionFlags.Standard);
      var parser = new ConstantsParser(builder);
      var _scanned = parser.Scan(state);
      if (_scanned)
      {
         var _symbol = builder.ToExpression().Map(e => e.Symbols[0]);
         if (_symbol)
         {
            if (_symbol.Value is IConstant c)
            {
               return c.Response();
            }
            else
            {
               return fail($"Expected constant, found {_symbol}");
            }
         }
         else
         {
            return _symbol.Exception;
         }
      }
      else
      {
         return _scanned.AnyException;
      }
   }

   public static BigInteger convert(string source, int baseValue, string possible)
   {
      source = source.Reverse();
      var accumulated = BigInteger.Zero;
      var bigBase = (BigInteger)baseValue;
      for (var exponent = 0; exponent < source.Length; exponent++)
      {
         var raised = BigInteger.Pow(bigBase, exponent);
         var index = possible.IndexOf(source[exponent]);
         accumulated += raised * index;
      }

      return accumulated;
   }

   public static double convertFloat(string source, int baseValue, string possible)
   {
      var left = convert(source.KeepUntil("."), baseValue, possible);

      var right = source.DropUntil(".").Drop(1);
      var accumulated = 0.0;
      for (var i = 0; i < right.Length; i++)
      {
         var exponent = i + 1;
         var raised = Math.Pow(baseValue, exponent);
         var index = possible.IndexOf(right[i]);
         accumulated += 1.0 / (raised / index);
      }

      return (double)left + accumulated;
   }

   public static Optional<Unit> getNumber(ExpressionBuilder builder, string type, string source)
   {
      switch (type)
      {
         case "":
            if (TryParse(source, out var integer))
            {
               builder.Add(new IntSymbol(integer));
               return unit;
            }
            else
            {
               return unableToConvert(source, "Int");
            }

         case "L":
            if (BigInteger.TryParse(source, out var bigInteger))
            {
               builder.Add(new LongSymbol(bigInteger));
               return unit;
            }
            else
            {
               return unableToConvert(source, "Long");
            }

         case "i":
            if (TryParse(source, out integer))
            {
               builder.Add(new ComplexSymbol(integer));
               return unit;
            }
            else
            {
               return unableToConvert(source, "Complex");
            }

         case "f":
            if (double.TryParse(source, out var real))
            {
               builder.Add(new FloatSymbol(real));
               return unit;
            }
            else
            {
               return unableToConvert(source, "Float");
            }

         default:
            return unableToConvert(source, "Int");
      }
   }

   public static Optional<Unit> getNumber(ExpressionBuilder builder, string type, BigInteger number)
   {
      switch (type)
      {
         case "":
            if (number < MinValue || number > MaxValue)
            {
               builder.Add(new LongSymbol(number));
               return unit;
            }
            else
            {
               builder.Add(new IntSymbol((int)number));
               return unit;
            }

         case "L":
            builder.Add(new LongSymbol(number));
            return unit;
         case "i":
            builder.Add(new ComplexSymbol((double)number));
            return unit;
         case "f":
            builder.Add(new FloatSymbol((double)number));
            return unit;
         default:
            return unableToConvert(number.ToString(), "Int");
      }
   }

   public static Responding<LambdaSymbol> getAnyLambda(ParseState state, Bits32<ExpressionFlags> flags)
   {
      var builder = new ExpressionBuilder(flags);
      var _scanned = new AnyLambdaParser(builder).Scan(state);
      if (_scanned)
      {
         var _symbol = builder.ToExpression().Map(expression => expression.Symbols[0]);
         if (_symbol)
         {
            return (LambdaSymbol)_symbol.Value;
         }
         else
         {
            return _symbol.Exception;
         }
      }
      else
      {
         return _scanned.AnyException;
      }
   }

   public static Optional<Maybe<LambdaSymbol>> getPossibleLambda(ParseState state, Bits32<ExpressionFlags> flags)
   {
      Maybe<LambdaSymbol> _none = nil;
      if (state.CurrentSource.StartsWith("("))
      {
         return _none.Response();
      }
      else
      {
         var _lambda = getAnyLambda(state, flags);
         if (_lambda)
         {
            return _lambda.Value.Some();
         }
         else if (_lambda.AnyException)
         {
            return _lambda.Exception;
         }
         else
         {
            return nil;
         }
      }
   }

   public static Optional<(Symbol, Expression, Maybe<Expression>)> getInnerComprehension(ParseState state) =>
      from comparisand in getValue(state, ExpressionFlags.Comparisand)
      from scanned in state.Scan("^ /(|s|) /':='", Color.Whitespace, Color.Structure)
      from source in getExpression(state, ExpressionFlags.OmitIf | ExpressionFlags.OmitComprehension)
      from ifExp in getIf(state)
      select (comparisand, source, ifExp);

   public static Optional<Maybe<Expression>> getIf(ParseState state)
   {
      var _scanned = state.Scan("^ /(|s+|) /'if' /b", Color.Whitespace, Color.Keyword);
      if (_scanned)
      {
         var _expression = getExpression(state, ExpressionFlags.OmitIf | ExpressionFlags.OmitComprehension);
         if (_expression)
         {
            return _expression.Value.Some();
         }
         else
         {
            return _expression.AnyException;
         }
      }
      else if (_scanned.AnyException)
      {
         return _scanned.Exception;
      }
      else
      {
         return nil;
      }
   }

   public static Optional<Maybe<Expression>> getAnd(ParseState state)
   {
      var builder = new ExpressionBuilder(ExpressionFlags.OmitIf);
      var parser = new IfAsAndParser(builder);
      var _scanned = parser.Scan(state);
      if (_scanned)
      {
         var _expression = builder.ToExpression();
         if (_expression)
         {
            return _expression.Value.Some();
         }
         else
         {
            return _expression.Exception;
         }
      }
      else if (_scanned.AnyException)
      {
         return _scanned.Exception;
      }
      else
      {
         return (Maybe<Expression>)nil;
      }
   }

   public static void addMatchElse(If ifStatement)
   {
      var current = ifStatement;
      var _nextIf = current.ElseIf;
      while (_nextIf)
      {
         current = _nextIf;
      }

      current.Else = new Block(new Return(new Expression(new ObjectSymbol(Unmatched.Value)), nil));
   }

   public static Optional<Maybe<AndSymbol>> andExpression(ParseState state)
   {
      var builder = new ExpressionBuilder(ExpressionFlags.Standard);
      var andParser = new IfAsAndParser(builder);
      var _scanned = andParser.Scan(state);
      if (_scanned)
      {
         var _andSymbol = builder.ToExpression().Map(e => (AndSymbol)e.Symbols[0]);
         if (_andSymbol)
         {
            return _andSymbol.Value.Some();
         }
         else
         {
            return _andSymbol.Exception;
         }
      }
      else if (_scanned.AnyException)
      {
         return _scanned.Exception;
      }
      else
      {
         return nil;
      }
   }

   public static Optional<Block> getCaseStatementBlock(ParseState state)
   {
      if (state.Scan("^ /(|s|) /'=' -(> '=')", Color.Whitespace, Color.Structure))
      {
         return getSingleLine(state, false);
      }
      else
      {
         return getBlock(state);
      }
   }

   public static Optional<Symbol> getOperator(ParseState state, string source, Bits32<ExpressionFlags> flags, bool whitespace)
   {
      Optional<Symbol> _symbol = nil;

      switch (source)
      {
         case "+":
            _symbol = new AddSymbol();
            break;
         case "-":
            _symbol = new SubtractSymbol();
            break;
         case "*":
            _symbol = new MultiplySymbol();
            break;
         case "/":
            _symbol = whitespace ? new FloatDivideSymbol() : new RationalSymbol();
            break;
         case "//":
            _symbol = new IntDivideSymbol();
            break;
         case "%":
            _symbol = new RemainderSymbol();
            break;
         case "%%":
            _symbol = new RemainderZeroSymbol(false);
            break;
         case "!%":
            _symbol = new RemainderZeroSymbol(true);
            break;
         case "^":
            _symbol = new RaiseSymbol();
            break;
         case "==":
            _symbol = new EqualSymbol();
            break;
         case "!=":
            _symbol = new NotEqualSymbol();
            break;
         case ">":
            _symbol = new GreaterThanSymbol();
            break;
         case ">=":
            _symbol = new GreaterThanEqualSymbol();
            break;
         case "<":
         {
            var _prefixCode = state.PrefixCode;
            if (_prefixCode)
            {
               _prefixCode.Value.Prefix();
               _symbol = new SpecialLessThanSymbol();
               state.PrefixCode = nil;
            }
            else
            {
               var lessThanSymbol = new LessThanSymbol();
               _symbol = lessThanSymbol;
               state.PrefixCode = lessThanSymbol;
            }

            break;
         }
         case "<=":
         {
            var _prefixCode = state.PrefixCode;
            if (_prefixCode)
            {
               _prefixCode.Value.Prefix();
               _symbol = new SpecialLessThanEqualSymbol();
               state.PrefixCode = nil;
            }
            else
            {
               var lessThanEqualSymbol = new LessThanEqualSymbol();
               _symbol = lessThanEqualSymbol;
               state.PrefixCode = lessThanEqualSymbol;
            }

            break;
         }
         case ";":
            _symbol = new SkipTakeOperatorPopSymbol();
            break;
         case "::":
            _symbol = new ConsSymbol();
            break;
         case "\\":
            _symbol = new FormatSymbol();
            break;
         case ",":
            if (flags[ExpressionFlags.OmitComma])
            {
               return nil;
            }
            else
            {
               state.Scan("^ /(/s*)", Color.Whitespace);
               _symbol = new CommaSymbol();
            }

            break;
         case "~":
            if (flags[ExpressionFlags.OmitConcatenate])
            {
               return nil;
            }
            else
            {
               _symbol = new ConcatenationSymbol();
            }

            break;
         case "<<":
         case ">>":
            _symbol = new SendBinaryMessageSymbol(source, Precedence.Shift);
            break;
         case "=>" when !flags[ExpressionFlags.OmitNameValue]:
            _symbol = new KeyValueSymbol();
            break;
         case "|>":
            _symbol = new PipelineSymbol();
            break;
         case "**":
            _symbol = new OpenRangeSymbol();
            break;
         case "<>":
            _symbol = new CompareSymbol();
            break;
         case "|":
            _symbol = new MatchSymbol();
            break;
         case "~~":
            _symbol = new SendBinaryMessageSymbol("matches(_<String>)", Precedence.Boolean, true);
            break;
         case "!~":
            _symbol = new SendBinaryMessageSymbol("notMatches(_<String>)", Precedence.Boolean, true);
            break;
         case "=~":
            _symbol = new SendBinaryMessageSymbol("isMatch(_<String>)", Precedence.Boolean, true);
            break;
         case ":-":
            _symbol = new BindSymbol();
            break;
         case "##":
            _symbol = new SendBinaryMessageSymbol("defaultTo(_)", Precedence.SendMessage);
            break;
      }

      return _symbol;
   }

   public static Optional<Expression> getTerm(ParseState state, ExpressionFlags flags)
   {
      var builder = new ExpressionBuilder(flags);
      var prefixParser = new PrefixParser(builder);
      var valuesParser = new ValuesParser(builder);
      var postfixParser = new PostfixParser(builder);

      while (state.More)
      {
         var _scanned = prefixParser.Scan(state);
         if (_scanned)
         {
         }
         else if (_scanned.AnyException)
         {
            return _scanned.Exception;
         }
         else
         {
            break;
         }
      }

      var _values = valuesParser.Scan(state);
      if (_values)
      {
      }
      else if (_values.AnyException)
      {
         return _values.Exception;
      }
      else
      {
         return invalidSyntax();
      }

      while (state.More)
      {
         var _scanned = postfixParser.Scan(state);
         if (_scanned)
         {
         }
         else if (_scanned.AnyException)
         {
            return _scanned.Exception;
         }
         else
         {
            break;
         }
      }

      return builder.ToExpression().Responding();
   }

   public static Optional<Block> getLambdaBlock(bool isExpression, ParseState state, Bits32<ExpressionFlags> flags,
      Maybe<TypeConstraint> _typeConstraint)
   {
      if (isExpression)
      {
         var _expression = getExpression(state, flags);
         return _expression.Map(e => new Block(new ExpressionStatement(e, true, _typeConstraint), _typeConstraint)
            { Index = state.Index });
      }
      else
      {
         return getBlock(state, _typeConstraint);
      }
   }

   public static Responding<(int, int, Maybe<Expression>, Maybe<Expression>)> getSkipTakeItem(ParseState state)
   {
      var parser = new SkipTakeItemParser();
      return parser.Scan(state).Map(_ => (parser.Skip, parser.Take, parser.Prefix, parser.Suffix));
   }

   public static Optional<SkipTakeItem[]> getSkipTakeItems(ParseState state)
   {
      var list = new List<SkipTakeItem>();
      while (state.More && getSkipTakeItem(state).If(out var tuple))
      {
         var (skip, take, prefix, suffix) = tuple;
         list.Add(new SkipTakeItem(skip, take, prefix, suffix));
         if (state.Scan("^ /'}'", Color.Structure).IsMatched)
         {
            break;
         }

         if (state.Scan("^ /(|s|) /',' /(|s|)", Color.Whitespace, Color.Structure, Color.Whitespace)
             .If(out _, out var anyException))
         {
         }
         else if (anyException.If(out var exception))
         {
            return failedMatch<SkipTakeItem[]>(exception);
         }
         else
         {
            return "Expected ,".FailedMatch<SkipTakeItem[]>();
         }
      }

      if (list.Count == 0)
      {
         return notMatched<SkipTakeItem[]>();
      }
      else
      {
         return list.ToArray().Matched();
      }
   }
}