using System.Numerics;
using Core.Collections;
using Core.Matching;
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
using Kagami.Library.Classes;
using static System.Int32;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;
using Array = System.Array;
using Complex = Kagami.Library.Objects.Complex;
using Group = System.Text.RegularExpressions.Group;
using Return = Kagami.Library.Nodes.Statements.Return;
using SkipTake = Kagami.Library.Parsers.Expressions.SkipTake;

namespace Kagami.Library.Parsers;

public static class ParserFunctions
{
   public const string REGEX_FIELD = "`?[A-Za-z_`][A-Za-z_0-9]*";
   public const string REGEX_PARAMETER = "`?[a-z_][A-Za-z_0-9]*";
   public const string REGEX_INVOKABLE = "`?[A-Za-z_][A-Za-z_0-9]*";
   public const string REGEX_INVOKABLE2 = @"`?[A-Za-z_][A-Za-z_0-9\$]*";
   public const string REGEX_CLASS = "[A-Z][A-Za-z_0-9]*";
   public const string REGEX_CLASS_OR_ALIAS = "[A-Za-z][A-Za-z_0-9]*";
   public const string REGEX_CLASS_GETTING = $@"{REGEX_CLASS}(?:\. {REGEX_CLASS})?";
   public const string REGEX_CLASS_GETTING_OR_ALIAS = $@"{REGEX_CLASS_OR_ALIAS}(?:\. {REGEX_CLASS_OR_ALIAS})?";
   public const string REGEX_ASSIGN_OPS = @"\+|-|\*|//|/%|/|/|\^|~|%|:\b";
   public const string REGEX_FUNCTION_NAME = $@"(?:(?:{REGEX_INVOKABLE})|(?:[~`!@\#\$%\^\*\+=\|\\;<>/\?&-]+)|\[\])=?(?![=>])";
   public const string REGEX_FUNCTION_NAME2 = $@"(?:(?:{REGEX_INVOKABLE2})|(?:[~`!@\#\$%\^\*\+=\|\\;<>/\?&-]+)|\[\])=?(?![=>])";
   public const string REGEX_SELECTOR = @$"(?:__\$)?{REGEX_FUNCTION_NAME}(?:\(.*\))?=?(?![=>])";
   public const string REGEX_EOL = @"\r\n|\r|\n|$";
   public const string REGEX_ANTICIPATE_END = $"(?=(?:{REGEX_EOL}))";
   public const string REGEX_OPERATORS = @"[-\+\*/\\%<=>!\.~\|\?\#@&\^,;:]";
   public const string REGEX_ITERATOR_FUNCTIONS = "sort|foldl|foldr|reducel|reducer|count|map|flatMap|bind|if|ifNot|index|indexes|min|max|" +
      "first|last|split|one|none|any|all|span|groupBy|for|while|until|z|zip|x|cross|fold|seq|takeWhile|takeUntil|skipWhile|" +
      @"skipUntil|!|\?|\*|@|\$";
   public const string REGEX_LIST_LEFT = @"\[:";
   public const string REGEX_LIST_RIGHT = @":\]";
   public const string REGEX_BLOCK_END = @"^(\s*)(\})";
   public const string REGEX_EXP_END = @"^(\s*)(\))";
   public const string REGEX_SINGLE_BLOCK = @"(=>)(?=[\w\s])";
   public const string REGEX_HIDDEN = @"(?:(hidden)\s+)?";

   public static StringSet keywords = ["do", "else", "true", "false", "return", "if"];

   public static bool isAKeyword(string word) => keywords.Contains(word);

   public static Optional<char> fromHex(string text)
   {
      var _char = $"0x{text}".FromHex();
      if (_char is (true, var @char))
      {
         return (char)@char;
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
      return getExpression(state, flags).Map(e => state.Scan(pattern, colors).Map(_ => e));
   }

   public static Optional<Expression> getExpression(ParseState state, string pattern, Bits32<ExpressionFlags> flags, Func<Group, int, Color> func)
   {
      return getExpression(state, flags).Map(e => state.Scan(pattern, func).Map(_ => e));
   }

   public static Optional<Expression> getCompoundComparisands(ParseState state, string fieldName, bool not)
   {
      var flags = ExpressionFlags.Comparisand | ExpressionFlags.OmitAnd | ExpressionFlags.OmitIf | ExpressionFlags.OmitAssign;
      var builder = new ExpressionBuilder(flags);

      var _comparisand = getExpression(state, flags);
      if (_comparisand is (true, var comparisand))
      {
         var specialComparisandIndex = comparisand.SpecialComparisandIndex;
         if (specialComparisandIndex == -1)
         {
            builder.Add(new FieldSymbol(fieldName));
            builder.Add(comparisand);
            builder.Add(new MatchSymbol(not));
         }
         else
         {
            if (comparisand.Symbols[specialComparisandIndex] is ISpecialComparisand specialComparisand)
            {
               specialComparisand.FieldName = fieldName;
            }

            builder.Add(comparisand);
         }

         var _scanned = state.Scan(@"^(\s*)(&)", Color.Whitespace, Color.Operator);
         if (_scanned)
         {
            return getCompoundComparisands(state, fieldName, false).Map(nextExpression =>
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
      else
      {
         return _comparisand.Exception;
      }
   }

   public static Optional<Sequence> getInternalList(ParseState state)
   {
      var builder = new ExpressionBuilder(ExpressionFlags.Standard);
      var constantsParser = new ConstantsParser(builder);

      while (state.More)
      {
         var _result = constantsParser.Scan(state);
         if (_result)
         {
            if (state.Scan(@"^(\s*)(,)", Color.Whitespace, Color.Operator))
            {
            }
            else if (_result.Exception is (true, var exception))
            {
               return exception;
            }
            else
            {
               break;
            }
         }
         else if (_result.Exception is (true, var exception))
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

         return new Sequence(list);
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
      "/%" => new DivRem(),
      "^" => new Raise(),
      "~" => new Concatenate(),
      "%" => new Remainder(),
      ":" => new NoOp(),
      _ => fail($"Didn't recognize operator {source}")
   };

   public static Optional<Block> getBlock(ParseState state, Maybe<TypeConstraint> _typeConstraint, bool checkForSemicolon = false)
   {
      if (checkForSemicolon)
      {
         var _semicolon = state.Scan(@"^(\s*)(;)", Color.Whitespace, Color.Structure);
         if (_semicolon)
         {
            return new Block();
         }
      }

      var _block = getRestOfLineBlock(state);
      if (_block)
      {
         return _block;
      }

      var _result = state.BeginBlock();
      if (_result)
      {
         var statementsParser = new StatementsParser();
         state.PushStatements();

         while (state.More)
         {
            var _endBlock = state.EndBlock();
            if (_endBlock)
            {
               break;
            }
            else if (_endBlock.Exception is (true, var exception))
            {
               return exception;
            }

            var _scanned = statementsParser.Scan(state);
            if (_scanned)
            {
            }
            else if (_scanned.Exception is (true, var exception))
            {
               return exception;
            }
            else
            {
               break;
            }
         }

         var _statements = state.PopStatements();
         if (_statements is (true, var statements))
         {
            return new Block(statements, _typeConstraint);
         }
         else
         {
            return nil;
         }
      }
      else if (_result.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return badBlock();
      }
   }

   public static Optional<Block> getBlock(ParseState state, bool semicolon = false) => getBlock(state, nil, semicolon);

   public static Optional<Block> getSingleLine(ParseState state, Maybe<TypeConstraint> _typeConstraint,
      bool returnExpression = true)
   {
      var statementsParser = new StatementsParser { ReturnExpression = returnExpression, TypeConstraint = _typeConstraint };
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

   public static Optional<Block> getSingleLineBlock(ParseState state, Maybe<TypeConstraint> _typeConstraint)
   {
      var _expression = getExpression(state, ExpressionFlags.Standard);
      if (_expression is (true, var expression))
      {
         var returnStatement = new Return(expression, _typeConstraint);
         return new Block(returnStatement, _typeConstraint);
      }
      else
      {
         return _expression.Exception;
      }
   }

   public static Optional<Symbol> getValue(ParseState state, Bits32<ExpressionFlags> flags)
   {
      var builder = new ExpressionBuilder(flags);
      var parser = new ValuesParser(builder);

      return parser.Scan(state).Map(_ => builder.Ordered.ToArray()[0]);
   }

   public static Optional<Parameters> getParameters(ParseState state)
   {
      var _scanned = state.Scan(@"^([\)\]])", Color.CloseParenthesis);
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
         if (_parameter is (true, var parameter))
         {
            if (parameter.DefaultValue)
            {
               defaultRequired = true;
            }

            parameters.Add(parameter);
            if (parameter.Variadic)
            {
               continuing = false;
            }
         }
         else
         {
            return _parameter.Exception;
         }

         var _next = state.Scan(@"^(\s*)([,\)])", Color.Whitespace, Color.CloseParenthesis);
         if (_next is (true, var next))
         {
            if (next.EndsWith(")"))
            {
               return new Parameters([.. parameters]);
            }
         }
         else
         {
            return _next.Exception;
         }

         if (!continuing)
         {
            return fail("There can be no parameters after a variadic parameter");
         }
      }

      return openParameters();
   }

   public static Optional<Parameters> getBlockParameters(ParseState state)
   {
      var _scanned = state.Scan(@"^(\|)", Color.Lambda);
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
         if (_parameter is (true, var parameter))
         {
            if (parameter.DefaultValue)
            {
               defaultRequired = true;
            }

            parameters.Add(parameter);
            if (parameter.Variadic)
            {
               continuing = false;
            }
         }
         else
         {
            return _parameter.Exception;
         }

         var _next = state.Scan(@"^(\s*)([,\|])", (g, i) => i switch
         {
            1 => Color.Whitespace,
            2 when g.Value.Contains(',') => Color.Structure,
            2 => Color.Lambda,
            _ => Color.Whitespace
         });
         if (_next is (true, var next))
         {
            if (next.EndsWith('|'))
            {
               return new Parameters([.. parameters]);
            }
         }
         else
         {
            return _next.Exception;
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
      var _scanned = state.Scan(@"^([\)\]\}])", Color.CloseParenthesis);
      if (_scanned)
      {
         return (Expression[])[];
      }
      else if (_scanned.Exception is (true, var exception))
      {
         return exception;
      }

      List<Expression> arguments = [];
      var scanning = true;

      while (state.More && scanning)
      {
         Bits32<ExpressionFlags> newFlags = flags | ExpressionFlags.InArgument;
         newFlags[ExpressionFlags.InSubExpression] = false;
         newFlags[ExpressionFlags.OmitComma] = true;
         var _expression = getExpression(state, newFlags);
         if (_expression is (true, var expression))
         {
            arguments.Add(expression);
            var _next = state.Scan(@"^(\s*)([,\)\]\}])", Color.Whitespace, Color.CloseParenthesis);
            if (_next is (true, var next))
            {
               if (next.EndsWith(')') || next.EndsWith(']') || next.EndsWith('}'))
               {
                  return (Expression[])[.. arguments];
               }
            }
            else
            {
               return _next.Exception;
            }
         }
         else if (_expression.Exception is (true, var exception))
         {
            return exception;
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
      var _arguments = getArguments(state, flags); // | ExpressionFlags.OmitColon);
      if (_arguments is (true, var arguments))
      {
         var _lambda = getPossibleLambda(state, flags);
         if (_lambda is (true, var lambda))
         {
            return (arguments, lambda);
         }
         else if (_lambda.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            return (arguments, nil);
         }
      }
      else
      {
         return _arguments.Exception;
      }
   }

   public static Optional<IObject> getComparisand(ParseState state)
   {
      var _expression = getExpression(state, ExpressionFlags.Comparisand | ExpressionFlags.OmitComma);
      if (_expression is (true, var expression))
      {
         if (expression.Symbols[0] is IConstant constant)
         {
            return constant.Object.Just();
         }
         else
         {
            return constantRequired(_expression);
         }
      }
      else
      {
         return _expression.Exception;
      }
   }

   public static Optional<IObject[]> getComparisandList(ParseState state)
   {
      var _scanned = state.Scan(@"^([\)\]])", Color.CloseParenthesis);
      if (_scanned)
      {
         return Array.Empty<IObject>();
      }
      else if (_scanned.Exception is (true, var exception))
      {
         return exception;
      }

      List<IObject> arguments = [];
      var scanning = true;

      while (state.More && scanning)
      {
         var _comparisand = getComparisand(state);
         if (_comparisand is (true, var comparisand))
         {
            arguments.Add(comparisand);
            var _next = state.Scan(@"^(\s*)([,\)\]])", Color.Whitespace, Color.CloseParenthesis);
            if (_next is (true, var next))
            {
               if (next.EndsWith(")") || next.EndsWith("]"))
               {
                  return arguments.ToArray();
               }
            }
            else
            {
               return _next.Exception;
            }
         }
         else if (_comparisand.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            scanning = false;
         }
      }

      return openArguments();
   }

   private static Optional<bool> parseHidden(ParseState state)
   {
      return state.Scan(@"^(\s*hidden\s+)?", Color.Keyword).Map(s => s.IsNotEmpty());
   }

   private static Optional<bool> parseReference(ParseState state)
   {
      return state.Scan(@"^(\s*ref\s+)?", Color.Keyword).Map(s => s.IsNotEmpty());
   }

   private static Optional<bool> parseMutable(ParseState state)
   {
      return state.Scan(@"^(\s*var\s+)?", Color.Keyword).Map(s => s.IsNotEmpty());
   }

   private static Optional<string> parseLabel(ParseState state)
   {
      return state.Scan($@"^(?:(\s*)({REGEX_FIELD})(:)(?!:))?", Color.Whitespace, Color.Label, Color.Structure)
         .Map(s => s.KeepUntil(":").Trim());
   }

   private static Optional<bool> parseNoCapturing(ParseState state)
   {
      return state.Scan(@"^(\s*nocap\s+)?", Color.Keyword).Map(s => s.IsNotEmpty());
   }

   private static Optional<string> parseParameterName(ParseState state)
   {
      return state.Scan(@$"^(\s*{REGEX_PARAMETER})\b", Color.Identifier).Map(s => s.Trim());
   }

   private static Optional<PossibleTypeConstraint> parseAliasedTypeConstraint(ParseState state)
   {
      var _alias = state.Scan(@"^( *)([a-z0-9]+)\b(?!\.)", 2, Color.Whitespace, Color.Keyword);
      return
         from alias in _alias
         from className in getClassNameFromAlias(alias)
         from baseClass in Module.Global.Value.Class(className)
         select (PossibleTypeConstraint)new PossibleTypeConstraint.Some(new TypeConstraint([baseClass]));
   }

   public static (string className, Color color) getClassNameWithColor(string source)
   {
      if (getClassNameFromAlias(source) is (true, var className))
      {
         return (className, Color.Keyword);
      }
      else
      {
         return (source, Color.Class);
      }
   }

   public static Optional<string> getClassNameFromAlias(string alias) => alias switch
   {
      "int" => "Int",
      "float" => "Float",
      "string" => "String",
      "char" => "Char",
      "byte" => "Byte",
      "bytes" => "ByteArray",
      "complex" => "Complex",
      "rational" => "Rational",
      "long" => "Long",
      "date" => "Date",
      "number" => "Number",
      "mstring" => "MutString",
      "lambda" => "Lambda",
      "bool" => "Boolean",
      "decimal" => "Decimal",
      "tuple" => "Tuple",
      "array" => "Array",
      "set" => "Set",
      "dict" => "Dictionary",
      "optional" => "Optional",
      "monad" => "Monad",
      "result" => "Result",
      "lazy" => "Lazy",
      "event" => "Event",
      _ => nil
   };

   public static Optional<string[]> getListOfClassNames(ParseState state)
   {
      List<string> classNames = [];

      while (state.More)
      {
         var _scanned = state.Scan(@$"^(\s*)({REGEX_CLASS_OR_ALIAS})", (g, i) => i switch
         {
            1 => Color.Whitespace,
            2 when g.Value.IsMatch("^ ['A-Z']") => Color.Class,
            2 => Color.Keyword,
            _ => Color.Whitespace
         });
         if (_scanned is (true, var name))
         {
            name = name.Trim();
            if (getClassNameFromAlias(name) is (true, var className))
            {
               classNames.Add(className);
            }
            else
            {
               classNames.Add(name);
            }
         }
         else if (_scanned.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            break;
         }
      }

      if (classNames.Count == 0)
      {
         return nil;
      }
      else
      {
         string[] result = [.. classNames];
         return result;
      }
   }

   public static Optional<PossibleTypeConstraint> parseUnionTypeConstraint(ParseState state)
   {
      return
         from begin in state.Scan("^( *)(<)", Color.Whitespace, Color.Class)
         from inner in getListOfClassNames(state)
         from end in state.Scan(@"^(\s*)(>)", Color.Whitespace, Color.Class)
         select (PossibleTypeConstraint)new PossibleTypeConstraint.Some(TypeConstraint.FromList(inner));
   }

   public static Optional<PossibleTypeConstraint> parseTypeConstraint(ParseState state)
   {
      var _result = parseUnionTypeConstraint(state);
      if (_result)
      {
         return _result;
      }
      else if (_result.Exception is (true, var exception))
      {
         return exception;
      }

      state.BeginTransaction();
      var _possibleTypeConstraint = parseAliasedTypeConstraint(state);
      if (_possibleTypeConstraint is (true, var possibleTypeConstraint))
      {
         state.CommitTransaction();
         return possibleTypeConstraint;
      }

      state.RollBackTransaction();
      var _className = state.Scan($@"^( *)({REGEX_CLASS})\b(?![\(\.])", Color.Whitespace, Color.Class)
         .Map(cn => cn.TrimStart());
      if (_className is (true, var className))
      {
         var _baseClass = Module.Global.Value.Class(className);
         if (_baseClass is (true, var baseClass))
         {
            return new PossibleTypeConstraint.Some(new TypeConstraint([baseClass]));
         }
         else if (Module.Global.Value.Forwarded(className))
         {
            return new PossibleTypeConstraint.Some(new TypeConstraint([new ForwardedClass(className)]));
         }
         else
         {
            return classNotFound(className);
         }
      }
      else if (_className.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         var builder = new ExpressionBuilder(ExpressionFlags.Standard);
         var typeConstraintParser = new TypeConstraintParser(builder);
         var _scanned = typeConstraintParser.Scan(state);
         if (_scanned)
         {
            var typeConstraint = (TypeConstraint)((IConstant)builder.Ordered.ToArray()[0]).Object;
            return new PossibleTypeConstraint.Some(typeConstraint);
         }
         else if (_scanned.Exception is (true, var exception2))
         {
            return exception2;
         }
         else
         {
            return new PossibleTypeConstraint.None();
         }
      }
   }

   private static Optional<bool> parseVaraidic(ParseState state)
   {
      var _scanned = state.Scan(@"^(\s*)(\.\.\.)", Color.Whitespace, Color.Structure);
      if (_scanned)
      {
         return true;
      }
      else if (_scanned.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return false;
      }
   }

   private static Optional<PossibleInvokable> parseDefaultValue(ParseState state, bool defaultRequired)
   {
      var _scanned = state.Scan(@"^(\s*=)(?!=)", Color.Structure);
      if (_scanned)
      {
         var _expression = getExpression(state, ExpressionFlags.OmitComma);
         if (_expression is (true, var expression))
         {
            var symbol = new InvokableExpressionSymbol(expression);
            state.AddSymbol(symbol);

            return new PossibleInvokable.Some(symbol.Invokable);
         }
         else if (_expression.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            return new PossibleInvokable.None();
         }
      }
      else if (_scanned.Exception is (true, var exception))
      {
         return exception;
      }
      else if (defaultRequired)
      {
         return fail("default required");
      }
      else
      {
         return new PossibleInvokable.None();
      }
   }

   private static Optional<Parameter> getParameter(ParseState state, bool defaultRequired) =>
      from hidden in parseHidden(state)
      from reference in parseReference(state)
      from mutable in parseMutable(state)
      from label in parseLabel(state)
      from noCapturing in parseNoCapturing(state)
      from name in parseParameterName(state)
      from typeConstraint in parseTypeConstraint(state)
      from variadic in parseVaraidic(state)
      from defaultValue in parseDefaultValue(state, defaultRequired)
      select new Parameter(hidden, mutable || reference, label, name, defaultValue, typeConstraint, reference, noCapturing) { Variadic = variadic };

   public static Optional<Block> getAnyBlock(ParseState state)
   {
      var _response = parseTypeConstraint(state);
      if (_response is (true, var response))
      {
         Maybe<TypeConstraint> _typeConstraint = response switch
         {
            PossibleTypeConstraint.Some some => some.TypeConstraint,
            _ => nil
         };
         state.SetReturnType(_typeConstraint);
         var _scanned = state.Scan($@"^(\s*){REGEX_SINGLE_BLOCK}", Color.Whitespace, Color.Block, Color.Whitespace);
         if (_scanned)
         {
            return getSingleLine(state, _typeConstraint);
         }
         else
         {
            return getBlock(state, _typeConstraint);
         }
      }
      else if (_response.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return nil;
      }
   }

   public static Optional<Block> getCaseReturnBlock(ParseState state)
   {
      var _response = parseTypeConstraint(state);
      if (_response is (true, var response))
      {
         Maybe<TypeConstraint> _typeConstraint = response switch
         {
            PossibleTypeConstraint.Some some => some.TypeConstraint,
            _ => nil
         };
         state.SetReturnType(_typeConstraint);
         var _scanned = state.Scan(@"^(\s*)" + REGEX_SINGLE_BLOCK, Color.Whitespace, Color.Block, Color.Whitespace);
         if (_scanned)
         {
            return getSingleLine(state, _typeConstraint);
         }
         else if (state.Scan(@"^(\s+)(return)(\s+)", Color.Whitespace, Color.Keyword, Color.Whitespace))
         {
            return getSingleLineBlock(state, _typeConstraint);
         }
         else
         {
            return getBlock(state, _typeConstraint);
         }
      }
      else if (_response.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return nil;
      }
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

      Optional<Unit> getLocalValue()
      {
         var _unit = valuesParser.Scan(state);
         if (_unit)
         {
            return unit;
         }
         else if (_unit.Exception is (true, var exception))
         {
            return exception;
         }

         _unit = unknownFieldParser.Scan(state);
         if (_unit)
         {
            maxFieldCount = unknownFieldParser.Index.MaxOf(maxFieldCount);
            addOne = true;

            return unit;
         }
         else if (_unit.Exception is (true, var exception))
         {
            return exception;
         }

         builder.Add(new FieldSymbol($"__${unknownFieldCount++}"));
         return _unit;
      }

      Optional<Unit> getLocalTerm()
      {
         var _unit = getLocalValue();
         if (_unit.Exception is (true, var exception))
         {
            return exception;
         }

         while (state.More)
         {
            _unit = postfixOperatorsParser.Scan(state);
            if (_unit.Exception is (true, var exception2))
            {
               return exception2;
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
      state.Scan(@"^(\s*)(\()", Color.Whitespace, Color.OpenParenthesis);

      try
      {
         while (state.More)
         {
            if (state.CurrentSource.StartsWith(")"))
            {
               break;
            }

            var _unit = getLocalTerm();
            if (_unit.Exception is (true, var exception))
            {
               return exception;
            }

            _unit = infixParser.Scan(state);
            if (_unit)
            {
               _unit = getLocalTerm();
               if (_unit.Exception is (true, var exception2))
               {
                  return exception2;
               }
            }
            else if (_unit.Exception is (true, var exception3))
            {
               return exception3;
            }
            else
            {
               break;
            }
         }

         var parameterCount = unknownFieldCount.MaxOf(maxFieldCount) + (addOne ? 1 : 0);
         var _scanned = state.Scan(@"^(\))", Color.CloseParenthesis);
         if (_scanned)
         {
            return builder.ToExpression().Map(expression => new LambdaSymbol(parameterCount, expression)).Optional();
         }
         else
         {
            return _scanned.Exception;
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
         if (_symbol is (true, var symbol))
         {
            if (symbol is IConstant c)
            {
               return c.Just();
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
         return _scanned.Exception;
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
         {
            if (TryParse(source, out var integer))
            {
               builder.Add(new IntSymbol(integer));
               return unit;
            }
            else if (BigInteger.TryParse(source, out var bigInteger))
            {
               builder.Add(new LongSymbol(bigInteger));
               return unit;
            }
            else
            {
               return unableToConvert(source, "Int");
            }
         }

         case "L":
         {
            if (BigInteger.TryParse(source, out var bigInteger))
            {
               builder.Add(new LongSymbol(bigInteger));
               return unit;
            }
            else
            {
               return unableToConvert(source, "Long");
            }
         }

         case "i":
         {
            if (TryParse(source, out var integer))
            {
               builder.Add(new ComplexSymbol(integer));
               return unit;
            }
            else
            {
               return unableToConvert(source, "Complex");
            }
         }

         case "f":
         {
            if (double.TryParse(source, out var real))
            {
               builder.Add(new FloatSymbol(real));
               return unit;
            }
            else
            {
               return unableToConvert(source, "Float");
            }
         }
         case "d":
         {
            if (decimal.TryParse(source, out var decimalValue))
            {
               builder.Add(new DecimalSymbol(decimalValue));
               return unit;
            }
            else
            {
               return unableToConvert(source, "Decimal");
            }
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
            }
            else
            {
               builder.Add(new IntSymbol((int)number));
            }

            return unit;

         case "L":
            builder.Add(new LongSymbol(number));
            return unit;
         case "i":
            builder.Add(new ComplexSymbol((double)number));
            return unit;
         case "f":
            builder.Add(new FloatSymbol((double)number));
            return unit;
         case "d":
            builder.Add(new DecimalSymbol((decimal)number));
            return unit;
         default:
            return unableToConvert(number.ToString(), "Int");
      }
   }

   public static Optional<IObject> getNumber(string type, BigInteger number)
   {
      switch (type)
      {
         case "":
            if (number > MaxValue || number < MinValue)
            {
               return Long.LongObject(number).Just();
            }
            else
            {
               return Int.IntObject((int)number).Just();
            }
         case "L":
            return Long.LongObject(number).Just();
         case "i":
            return new Complex((double)number);
         case "f":
         {
            try
            {
               return Float.FloatObject((double)number).Just();
            }
            catch
            {
               return fail("Can't convert to Float");
            }
         }
         case "d":
         {
            try
            {
               return KDecimal.KDecimalObject((decimal)number).Just();
            }
            catch
            {
               return fail("Can't convert to Decimal");
            }
         }

         default:
            return fail("Unable to convert");
      }
   }

   public static Optional<LambdaSymbol> getAnyLambda(ParseState state, Bits32<ExpressionFlags> flags)
   {
      var builder = new ExpressionBuilder(flags);
      var _scanned = new AnyLambdaParser(builder).Scan(state);
      if (_scanned)
      {
         if (builder.Length == 0)
         {
            return fail("No lambda found");
         }

         var _symbol = builder.ToExpression().Map(expression => expression.Symbols[0]);
         if (_symbol is (true, var symbol))
         {
            return (LambdaSymbol)symbol;
         }
         else
         {
            return _symbol.Exception;
         }
      }
      else
      {
         return _scanned.Exception;
      }
   }

   public static Optional<LambdaSymbol> getPossibleLambda(ParseState state, Bits32<ExpressionFlags> flags)
   {
      if (state.CurrentSource.StartsWith('('))
      {
         return nil;
      }
      else
      {
         return getAnyLambda(state, flags);
      }
   }

   public static Optional<(Symbol, Expression, PossibleExpression)> getComprehensionBody(ParseState state) =>
      from comparisand in getValue(state, ExpressionFlags.Comparisand | ExpressionFlags.OmitIn)
      from scanned in state.Scan(@"^(\s+)(in)", Color.Whitespace, Color.Keyword)
      from source in getExpression(state, ExpressionFlags.OmitIf | ExpressionFlags.OmitIn | ExpressionFlags.OmitComprehension)
      from ifExp in getIf(state)
      select (comparisand, source, ifExp);

   public static Optional<PossibleExpression> getIf(ParseState state)
   {
      var _scanned = state.Scan(@"^(\s+)(if)\b", Color.Whitespace, Color.Keyword);
      if (_scanned)
      {
         var _expression = getExpression(state, ExpressionFlags.OmitIf | ExpressionFlags.OmitComprehension);
         if (_expression is (true, var expression))
         {
            return new PossibleExpression.Some(expression);
         }
         else
         {
            return _expression.Exception;
         }
      }
      else if (_scanned.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return new PossibleExpression.None();
      }
   }

   public static Optional<PossibleIfExpression> getIfOrIfNo(ParseState state)
   {
      var _scanned = state.Scan(@"^(\s+)(if)\b", Color.Whitespace, Color.Keyword);
      if (_scanned)
      {
         var not = state.NotKeyword();
         var _expression = getExpression(state, ExpressionFlags.OmitIf | ExpressionFlags.OmitComprehension);
         if (_expression is (true, var expression))
         {
            return not ? new PossibleIfExpression.IfNot(expression) : new PossibleIfExpression.If(expression);
         }
         else
         {
            return _expression.Exception;
         }
      }
      else if (_scanned.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return new PossibleIfExpression.None();
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
         if (_expression is (true, var expression))
         {
            return expression.Some();
         }
         else
         {
            return _expression.Exception;
         }
      }
      else if (_scanned.Exception is (true, var exception))
      {
         return exception;
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
      while (_nextIf is (true, var nextIf))
      {
         current = nextIf;
         _nextIf = nextIf.ElseIf;
      }

      current.Else = new Block(new FailedMatch());
   }

   public static Optional<PossibleAndSymbol> andExpression(ParseState state)
   {
      var builder = new ExpressionBuilder(ExpressionFlags.Standard | ExpressionFlags.OmitLambda);
      var andParser = new IfAsAndParser(builder);
      var _scanned = andParser.Scan(state);
      if (_scanned)
      {
         var _andSymbol = builder.ToExpression().Map(e => (AndSymbol)e.Symbols[0]);
         if (_andSymbol is (true, var andSymbol))
         {
            return new PossibleAndSymbol.Some(andSymbol);
         }
         else
         {
            return _andSymbol.Exception;
         }
      }
      else if (_scanned.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return new PossibleAndSymbol.None();
      }
   }

   public static Optional<Block> getCaseStatementBlock(ParseState state)
   {
      if (state.Scan(@"^(\s*)" + REGEX_SINGLE_BLOCK, Color.Whitespace, Color.Block))
      {
         return getSingleLine(state, false);
      }
      else if (state.Scan(@"^(\s*)(return)(\s*)", Color.Whitespace, Color.Keyword, Color.Whitespace))
      {
         return getSingleLineBlock(state, nil);
      }
      else
      {
         return getBlock(state);
      }
   }

   public static Optional<Symbol> getOperator(ParseState state, string source, Bits32<ExpressionFlags> flags)
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
            _symbol = new FloatDivideSymbol();
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
            state.PrefixCode = nil;
            break;
         case "!=":
            _symbol = new NotEqualSymbol();
            state.PrefixCode = nil;
            break;
         case ">":
            _symbol = new GreaterThanSymbol();
            state.PrefixCode = nil;
            break;
         case ">=":
            _symbol = new GreaterThanEqualSymbol();
            state.PrefixCode = nil;
            break;
         case "<":
            _symbol = new LessThanSymbol();
            break;
         case "<=":
            _symbol = new LessThanEqualSymbol();
            break;
         case "::" when flags[ExpressionFlags.Comparisand]:
            _symbol = new ConsObjectSymbol();
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
               state.Scan(@"^(\s*)", Color.Whitespace);
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
            _symbol = new SendBinaryMessageSymbol($"{source}(_)", Precedence.Shift);
            break;
         case "|>" when !flags[ExpressionFlags.InLambda]:
            _symbol = new PipelineSymbol();
            break;
         case "<|" when !flags[ExpressionFlags.InLambda]:
            _symbol = new BackPipelineSymbol();
            break;
         case "...":
            _symbol = new OpenRangeSymbol();
            break;
         case "<>":
            _symbol = new CompareSymbol();
            break;
         case "||":
            _symbol = new MatchSymbol(false);
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
         case ":-" when !flags[ExpressionFlags.OmitBind]:
            _symbol = new BindSymbol();
            break;
         /*case "//":
            _symbol = new RationalSymbol();
            break;*/
         /*case "<|":
            _symbol = new SendBinaryMessageSymbol("<|(_)", Precedence.Shift);
            break;*/
         case "/:":
            _symbol = new SendBinaryMessageSymbol("foldl(_)", Precedence.ChainedOperator);
            break;
         case "\\:":
            _symbol = new SendBinaryMessageSymbol("foldr(_)", Precedence.ChainedOperator);
            break;
         /*case "=>":
            _symbol = new SendBinaryMessageSymbol("map(_)", Precedence.ChainedOperator);
            break;
         case "??":
            _symbol = new SendBinaryMessageSymbol("if(_)", Precedence.ChainedOperator);
            break;*/
         case "|<<":
            _symbol = new SendBinaryMessageSymbol("|<<(_)", Precedence.Shift);
            break;
         case "+++":
            _symbol = new IncrementSymbol();
            break;
         case "---":
            _symbol = new DecrementSymbol();
            break;
         case "$":
            _symbol = new SendBinaryMessageSymbol("slice(_)", Precedence.PostfixOperator);
            break;
         case "**":
            _symbol = new RepeatAsArraySymbol();
            break;
         case "===":
            _symbol = new SendBinaryMessageSymbol("accept(_)", Precedence.Boolean);
            break;
         case "//":
            _symbol = new IntDivideSymbol();
            break;
         case "/%":
            _symbol = new DivModSymbol();
            break;
         case @"\\":
            _symbol = new ForcedFloatDivide();
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
         else if (_scanned.Exception is (true, var exception))
         {
            return exception;
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
      else if (_values.Exception is (true, var exception))
      {
         return exception;
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
         else if (_scanned.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            break;
         }
      }

      return builder.ToExpression().Optional();
   }

   public static Optional<Block> getLambdaBlock(bool isExpression, bool isSingleLine, ParseState state, Bits32<ExpressionFlags> flags,
      Maybe<TypeConstraint> _typeConstraint)
   {
      if (isExpression)
      {
         if (isSingleLine)
         {
            return getSingleLine(state, _typeConstraint);
         }
         else
         {
            var _expression = getExpression(state, flags);
            return _expression.Map(e => new Block(new ExpressionStatement(e, true, _typeConstraint), _typeConstraint)
               { Index = state.Index });
         }
      }
      else
      {
         return getBlock(state, _typeConstraint);
      }
   }

   public static Optional<SkipTake> getSkipTake(ParseState state, ExpressionFlags flags)
   {
      var skipTake = new SkipTake();

      var _noSkipMatch = state.Scan(@"^(\s*)(:)", Color.Whitespace, Color.Operator);
      if (_noSkipMatch)
      {
      }
      else if (_noSkipMatch.Exception is (true, var noSkipMatchException))
      {
         return noSkipMatchException;
      }
      else
      {
         var _skipExpression = getExpression(state, flags);
         if (_skipExpression is (true, var skipExpression))
         {
            skipTake.Skip = skipExpression;
         }
         else if (_skipExpression.Exception is (true, var skipExpressionException))
         {
            return skipExpressionException;
         }

         var _semiOrEnd = state.Scan(@"^(\s*)([;:}])", colorize);
         if (_semiOrEnd is (true, var semiOrEnd))
         {
            switch (semiOrEnd)
            {
               case "}":
                  skipTake.Terminal = true;
                  return skipTake;
               case ";":
                  return skipTake;
            }
         }
         else if (_semiOrEnd.Exception is (true, var semiOrEndException))
         {
            return semiOrEndException;
         }
      }

      if (!state.CurrentSource.IsMatch("^ /s* ['};']"))
      {
         var _takeExpression = getExpression(state, flags);
         if (_takeExpression is (true, var takeExpression))
         {
            skipTake.Take = takeExpression;
         }
         else if (_takeExpression.Exception is (true, var exception))
         {
            return exception;
         }
      }

      var _end = state.Scan(@"^(\s*)([};])", colorize);
      if (_end is (true, var end))
      {
         switch (end)
         {
            case "}":
               skipTake.Terminal = true;
               return skipTake;
         }
      }
      else if (_end.Exception is (true, var exception4))
      {
         return exception4;
      }

      return skipTake;

      Color colorize(Group g, int i) => i switch
      {
         1 => Color.Whitespace,
         2 when g.Value == "}" => Color.CloseParenthesis,
         _ => Color.Operator
      };
   }

   public static Optional<Block> getPartialBlock(ParseState state, Color endBlockColor = Color.Block) => getPartialBlock(state, nil, endBlockColor);

   public static Optional<Block> getPartialBlock(ParseState state, Maybe<TypeConstraint> _typeConstraint, Color endBlockColor = Color.Block)
   {
      var statementsParser = new StatementsParser();
      state.PushStatements();

      while (state.More)
      {
         var _endBlock = state.EndBlock(endBlockColor);
         if (_endBlock)
         {
            break;
         }
         else if (_endBlock.Exception is (true, var exception))
         {
            return exception;
         }

         var _scanned = statementsParser.Scan(state);
         if (_scanned)
         {
         }
         else if (_scanned.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            break;
         }
      }

      var _statements = state.PopStatements();
      if (_statements is (true, var statements))
      {
         return new Block(statements, _typeConstraint);
      }
      else
      {
         return nil;
      }
   }

   public static Optional<Block> getRestOfLineBlock(ParseState state)
   {
      var _scanned = state.Scan("^( +)(?!{)", Color.Whitespace);
      if (_scanned)
      {
         state.PushStatements();
         var statementsParser = new StatementsParser();
         var _unitScanned = statementsParser.Scan(state);
         if (_unitScanned && state.PopStatements() is (true, var statements))
         {
            return new Block(statements);
         }
      }

      return nil;
   }

   public static Optional<PossibleBlock> getElse(ParseState state)
   {
      var _block =
         from scanned in state.Scan(@"^(\s*)(else)\b", Color.Whitespace, Color.Keyword)
         from blockValue in getBlock(state)
         select blockValue;
      if (_block is (true, var block))
      {
         return new PossibleBlock.Some(block);
      }
      else if (_block.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return new PossibleBlock.None();
      }
   }

   public static Exception upToEndOfLine(ParseState state)
   {
      var _scanned = state.Scan(@"^(\s*)(\S+)");
      return fail($"Didn't understand: \"{_scanned | (() => state.CurrentSource)}\"");
   }

   public static Optional<TaggedExpression[]> getTaggedExpressions(ParseState state, string ending)
   {
      List<TaggedExpression> taggedExpressions = [];
      while (state.More)
      {
         var _end = state.Scan(ending, Color.Whitespace, Color.CloseParenthesis);
         if (_end)
         {
            break;
         }

         var _tag = state.Scan($@"^(\s*)({REGEX_FIELD})(\s*)(=)", 2, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure);
         if (_tag is (true, var tag))
         {
            var _expression = getExpression(state, ExpressionFlags.Standard | ExpressionFlags.OmitComma);
            if (_expression is (true, var expression))
            {
               taggedExpressions.Add(new TaggedExpression(tag, expression));
            }
            else if (_expression.Exception is (true, var exception))
            {
               return exception;
            }
            else
            {
               break;
            }
         }
         else
         {
            return _tag.Exception;
         }

         state.Scan(@"^(\s*)(,)", Color.Whitespace, Color.Structure);
      }

      TaggedExpression[] result = [.. taggedExpressions];
      return result;
   }

   public static Optional<Expression[]> getExpressions(ParseState state, string ending)
   {
      List<Expression> expressions = [];

      while (state.More)
      {
         var _end = state.Scan(ending, Color.Whitespace, Color.CloseParenthesis);
         if (_end)
         {
            break;
         }

         var _expression = getExpression(state, ExpressionFlags.OmitComma);
         if (_expression is (true, var expression))
         {
            expressions.Add(expression);
         }
         else
         {
            return _expression.Exception;
         }

         state.Scan(@"^(\s*)(,)", Color.Whitespace, Color.Structure);
      }

      Expression[] result = [.. expressions];
      return result;
   }

   public static Optional<Block> getReturnExpression(ParseState state)
   {
      return
         from arrow in state.Scan($@"^(\s*){REGEX_SINGLE_BLOCK}", Color.Whitespace, Color.Block)
         from expression in getExpression(state, ExpressionFlags.Standard)
         select new Block(expression);
   }

   public static Optional<PossibleBlock> getExitBlock(ParseState state)
   {
      var _block =
         from keyword in state.Scan(@"^(\s*)(exit)\b", Color.Whitespace, Color.Keyword)
         from blockValue in getBlock(state)
         select blockValue;
      if (_block is (true, var block))
      {
         return new PossibleBlock.Some(block);
      }
      else if (_block.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return new PossibleBlock.None();
      }
   }
}