﻿using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Expressions;
using Kagami.Library.Parsers.Statements;
using Standard.Types.Maybe;
using Standard.Types.Numbers;
using Standard.Types.Strings;
using static System.Int32;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;
using Return = Kagami.Library.Nodes.Statements.Return;

namespace Kagami.Library.Parsers
{
   public static class ParserFunctions
   {
      public const string REGEX_FIELD = "['A-Za-z_'] ['A-Za-z_0-9']*";
      public const string REGEX_INVOKABLE = "['A-Za-z_'] ['A-Za-z_0-9']*";
      public const string REGEX_CLASS = "['A-Z'] ['A-Za-z_0-9']*";
      public const string REGEX_CLASS_GETTING = REGEX_CLASS + "('.' " + REGEX_CLASS + ")?";
      public const string REGEX_ASSIGN_OPS = "'+' | '-' | '*' | '////' | '//' | '%' | '^'";
      public const string REGEX_FUNCTION_NAME = "((" + REGEX_INVOKABLE + ") | (['~`!@#$%^&*+=|\\;<>//?-']+) | '[]') '='?";
      public const string REGEX_EOL = "/r /n | /r | /n";
      public const string REGEX_ANTICIPATE_END = "(> " + REGEX_EOL + ")";
      public const string REGEX_OPERATORS = "['-+*//\\%<=>!.~|?#@&^,.:']";

      public static IMatched<char> fromHex(string text)
      {
         return $"0x{text}".FromHex().Result($"Didn't understand {text}").FlatMap(i => ((char)i).Matched(), failedMatch<char>);
      }

      public static IResult<char> fromBackslash(char original)
      {
         switch (original)
         {
            case 'n':
               return '\n'.Success();
            case 'r':
               return '\r'.Success();
            case 't':
               return '\t'.Success();
            default:
               return $"Didn't understand {original}".Failure<char>();
         }
      }

      public static IMatched<Expression> getExpression(ParseState state, Bits32<ExpressionFlags> flags)
      {
         var expressionParser = new ExpressionParser(flags);
         return expressionParser.Scan(state).Map(u => expressionParser.Expression);
      }

      public static IMatched<Expression> getExpression(ParseState state, string pattern, Bits32<ExpressionFlags> flags,
         params Color[] colors)
      {
         return getExpression(state, flags).Map(e => state.Scan(pattern, colors).Map(s => e));
      }

      public static IMatched<InternalList> getInternalList(ParseState state)
      {
         var builder = new ExpressionBuilder(ExpressionFlags.Standard);
         var constantsParser = new ConstantsParser(builder);

         while (state.More)
         {
            var result = constantsParser.Scan(state);
            if (result.IsMatched)
               if (state.Scan("^ /(|s|) /','", Color.Whitespace, Color.Operator).IsMatched) { }
               else if (result.Failed(out var exception))
                  return failedMatch<InternalList>(exception);
               else
                  break;
            else if (result.Failed(out var exception))
               return failedMatch<InternalList>(exception);
            else
               break;
         }

         if (builder.ToExpression().If(out var expression, out var expException))
         {
            var symbols = expression.Symbols;
            var list = new List<IObject>();
            foreach (var symbol in symbols)
               if (symbol is IConstant c)
                  list.Add(c.Object);
               else
                  return $"Expected constant, found {symbol}".FailedMatch<InternalList>();

            return new InternalList(list).Matched();
         }
         else
            return failedMatch<InternalList>(expException);
      }

      public static IMatched<Operation> matchOperator(string source)
      {
         switch (source)
         {
            case "":
               return notMatched<Operation>();
            case "+":
               return new Add().Matched<Operation>();
            case "-":
               return new Subtract().Matched<Operation>();
            case "*":
               return new Multiply().Matched<Operation>();
            case "/":
               return new FloatDivide().Matched<Operation>();
            case "//":
               return new IntDivide().Matched<Operation>();
            case "%":
               return new Remainder().Matched<Operation>();
            case "^":
               return new Raise().Matched<Operation>();
            default:
               return $"Didn't recognize operator {source}".FailedMatch<Operation>();
         }
      }

      public static IMatched<Block> getBlock(ParseState state)
      {
         if (state.Advance().If(out _, out var isNotMatched, out var exception))
         {
            var statementsParser = new StatementsParser();
            state.PushStatements();
            while (state.More)
            {
               var (type, _, statementException) = statementsParser.Scan(state).Values;
               if (type == MatchType.NotMatched)
                  break;

               if (type == MatchType.FailedMatch)
                  return failedMatch<Block>(statementException);
            }

            if (state.PopStatements().If(out var statements, out exception))
            {
               state.Regress();
               return new Block(statements).Matched();
            }
            else
               return failedMatch<Block>(exception);
         }
         else if (isNotMatched)
            return failedMatch<Block>(badIndentation());
         else
            return failedMatch<Block>(exception);
      }

      public static IMatched<Block> getSingleLine(ParseState state, bool returnExpression = true)
      {
         var statementsParser = new StatementsParser(true) { ReturnExpression = returnExpression };
         state.PushStatements();
         if (statementsParser.Scan(state).If(out _, out var isNotMatched, out var exception))
            if (state.PopStatements().If(out var statements, out exception))
               return new Block(statements).Matched();
            else
               return failedMatch<Block>(exception);
         else if (isNotMatched)
         {
            state.PopStatements();
            return notMatched<Block>();
         }
         else
            return failedMatch<Block>(exception);
      }

      public static IMatched<Symbol> getValue(ParseState state, Bits32<ExpressionFlags> flags)
      {
         var builder = new ExpressionBuilder(flags);
         var parser = new ValuesParser(builder);
         return parser.Scan(state).Map(u => builder.Ordered.ToArray()[0]);
      }

      public static IMatched<Parameters> getParameters(ParseState state)
      {
         if (state.Scan("^ /[')]']", Color.Structure).If(out _, out var isNotMatched, out var scanException))
            return new Parameters().Matched();
         else if (!isNotMatched)
            return failedMatch<Parameters>(scanException);

         var parameters = new List<Parameter>();
         var defaultRequired = false;
         var continuing = true;

         while (state.More && continuing)
         {
            var (type, parameter, exception) = getParameter(state, defaultRequired).Values;
            switch (type)
            {
               case MatchType.Matched:
                  if (parameter.DefaultValue.IsSome)
                     defaultRequired = true;
                  parameters.Add(parameter);
                  if (parameter.Variadic)
                     continuing = false;
                  break;
               case MatchType.NotMatched:
                  return notMatched<Parameters>();
               case MatchType.FailedMatch:
                  return failedMatch<Parameters>(exception);
            }

            var (nextType, next, nextException) = state.Scan("^ /(/s*) /[',)']", Color.Whitespace, Color.Structure).Values;
            switch (nextType)
            {
               case MatchType.Matched:
                  if (next.EndsWith(")"))
                     return new Parameters(parameters.ToArray()).Matched();

                  break;
               case MatchType.NotMatched:
                  return notMatched<Parameters>();
               case MatchType.FailedMatch:
                  return failedMatch<Parameters>(nextException);
            }

            if (!continuing)
               return "There can be no parameters after a variadic parameter".FailedMatch<Parameters>();
         }

         return failedMatch<Parameters>(openParameters());
      }

      public static IMatched<Expression[]> getArguments(ParseState state, Bits32<ExpressionFlags> flags)
      {
         if (state.Scan("^ /')'", Color.Structure).If(out _, out var isNotMatched, out var scanException))
            return new Expression[0].Matched();
         else if (!isNotMatched)
            return failedMatch<Expression[]>(scanException);

         var arguments = new List<Expression>();
         var scanning = true;

         while (state.More && scanning)
         {
            var (type, expression, exception) = getExpression(state, flags | ExpressionFlags.OmitComma).Values;
            switch (type)
            {
               case MatchType.Matched:
                  arguments.Add(expression);
                  var (nextType, next, nextException) =
                     state.Scan("^ /(/s*) /[',)]']", Color.Whitespace, Color.Structure).Values;
                  switch (nextType)
                  {
                     case MatchType.Matched:
                        if (next.EndsWith(")") || next.EndsWith("]"))
                           return arguments.ToArray().Matched();

                        break;
                     case MatchType.NotMatched:
                        return notMatched<Expression[]>();
                     case MatchType.FailedMatch:
                        return failedMatch<Expression[]>(nextException);
                  }

                  break;
               case MatchType.NotMatched:
                  scanning = false;
                  break;
               case MatchType.FailedMatch:
                  return failedMatch<Expression[]>(exception);
            }
         }

         return failedMatch<Expression[]>(openArguments());
      }

      public static IMatched<(Expression[], IMaybe<LambdaSymbol>)> getArgumentsPlusLambda(ParseState state,
         Bits32<ExpressionFlags> flags) =>
         from arguments in getArguments(state, flags)
         from lambda in getPossibleLambda(state, flags)
         select (arguments, lambda);

      public static IMatched<IObject> getComparisand(ParseState state)
      {
         if (getExpression(state, ExpressionFlags.Comparisand | ExpressionFlags.OmitComma).If(out var expression, out var original))
            if (expression.Symbols[0] is IConstant constant)
               return constant.Object.Matched();
            else
               return failedMatch<IObject>(constantRequired(expression));
         else
            return original.Unmatched<IObject>();
      }

      public static IMatched<IObject[]> getComparisandList(ParseState state)
      {
         if (state.Scan("^ /[')]']", Color.Structure).If(out _, out var isNotMatched, out var scanException))
            return new IObject[0].Matched();
         else if (!isNotMatched)
            return failedMatch<IObject[]>(scanException);

         var arguments = new List<IObject>();
         var scanning = true;

         while (state.More && scanning)
         {
            var (type, comparisand, exception) = getComparisand(state).Values;
            switch (type)
            {
               case MatchType.Matched:
                  arguments.Add(comparisand);
                  var (nextType, next, nextException) =
                     state.Scan("^ /(/s*) /[',)]']", Color.Whitespace, Color.Structure).Values;
                  switch (nextType)
                  {
                     case MatchType.Matched:
                        if (next.EndsWith(")") || next.EndsWith("]"))
                           return arguments.ToArray().Matched();

                        break;
                     case MatchType.NotMatched:
                        return notMatched<IObject[]>();
                     case MatchType.FailedMatch:
                        return failedMatch<IObject[]>(nextException);
                  }

                  break;
               case MatchType.NotMatched:
                  scanning = false;
                  break;
               case MatchType.FailedMatch:
                  return failedMatch<IObject[]>(exception);
            }
         }

         return failedMatch<IObject[]>(openArguments());
      }

      static IMatched<bool> parseReference(ParseState state)
      {
         return state.Scan("^ /(/s* 'ref' /s+)?", Color.Keyword).Map(s => s.IsNotEmpty());
      }

      static IMatched<bool> parseMutable(ParseState state)
      {
         return state.Scan("^ /(/s* 'var' /s+)?", Color.Keyword).Map(s => s.IsNotEmpty());
      }

      static IMatched<string> parseExternalName(ParseState state)
      {
         return state.Scan($"^ (/(/s*) /({REGEX_FIELD}) /':')?", Color.Whitespace, Color.Label, Color.Structure)
            .Map(s => s.TakeUntil(":").Trim());
      }

      static IMatched<string> parseParameterName(ParseState state)
      {
         return state.Scan($"^ /(/s* {REGEX_FIELD}) /b", Color.Identifier).Map(s => s.Trim());
      }

      static IMatched<bool> parseVaraidic(ParseState state)
      {
         if (state.Scan("^ /(|s|) /'...'", Color.Whitespace, Color.Structure).If(out _, out var isNotMatched, out var exception))
            return true.Matched();
         else if (isNotMatched)
            return false.Matched();
         else
            return failedMatch<bool>(exception);
      }

      static IMatched<IMaybe<IInvokable>> parseDefaultValue(ParseState state, bool defaultRequired)
      {
         var result = state.Scan("^ /(/s* '=') -(> '=')", Color.Structure);
         var (type, _, exception) = result.Values;
         switch (type)
         {
            case MatchType.Matched:
               return getExpression(state, ExpressionFlags.OmitComma).Map(e =>
               {
                  var symbol = new InvokableExpressionSymbol(e);
                  state.AddSymbol(symbol);
                  return symbol.Invokable.Some().Matched();
               });
            case MatchType.NotMatched:
               if (defaultRequired)
                  return "default required".FailedMatch<IMaybe<IInvokable>>();

               return none<IInvokable>().Matched();
            default:
               return failedMatch<IMaybe<IInvokable>>(exception);
         }
      }

      static IMatched<Parameter> getParameter(ParseState state, bool defaultRequired) =>
         from reference in parseReference(state)
         from mutable in parseMutable(state)
         from externalName in parseExternalName(state)
         from name in parseParameterName(state)
         from variadic in parseVaraidic(state)
         from defaultValue in parseDefaultValue(state, defaultRequired)
         select new Parameter(mutable, externalName, name, defaultValue, reference) { Variadic = variadic };

      public static IMatched<Block> getAnyBlock(ParseState state)
      {
         if (state.Scan("^ /(|s|) /'=' /(|s|)", Color.Whitespace, Color.Structure, Color.Whitespace).IsMatched)
            return getSingleLine(state);
         else
            return getBlock(state);
      }

      public static IResult<If> getIf(string parameterName, Symbol comparisand, IMaybe<Expression> and, Block block)
      {
         var builder = new ExpressionBuilder(ExpressionFlags.Standard);
         builder.Add(new FieldSymbol(parameterName));
         builder.Add(new SendMessageSymbol("match", Precedence.SendMessage, new Expression(comparisand)));
         if (and.If(out var andExpression))
            builder.Add(andExpression);

         return builder.ToExpression().Map(expression => new If(expression, block));
      }

      public static IMatched<LambdaSymbol> getPartialLambda(ParseState state)
      {
         if (!state.More)
            return notMatched<LambdaSymbol>();

         var unknownFieldCount = 0;
         var maxFieldCount = 0;
         var addOne = false;
         var builder = new ExpressionBuilder(ExpressionFlags.OmitComma);
         var unknownFieldParser = new UnknownFieldParser(builder);
         var valuesParser = new ValuesParser(builder);
         var postfixOperatorsParser = new PostfixOperatorsParser(builder);
         var infixParser = new InfixParser(builder);

         IMatched<Unit> getValue()
         {
            if (valuesParser.Scan(state).If(out _, out var original))
               return Unit.Matched();
            else if (original.IsFailedMatch)
               return original;

            if (unknownFieldParser.Scan(state).If(out _, out original))
            {
               maxFieldCount = unknownFieldParser.Index.MaxOf(maxFieldCount);
               addOne = true;
               return Unit.Matched();
            }
            else if (original.IsFailedMatch)
               return original;

            builder.Add(new FieldSymbol($"{unknownFieldCount++}".get()));
            return Unit.Matched();
         }

         IMatched<Unit> getTerm()
         {
            if (getValue().If(out _, out var original)) { }
            else if (original.IsFailedMatch)
               return original;

            while (state.More)
               if (postfixOperatorsParser.Scan(state).If(out _, out var isNotMatched, out var exception)) { }
               else if (isNotMatched)
                  break;
               else
                  return failedMatch<Unit>(exception);

            return Unit.Matched();
         }

         state.Scan("^ /(|s|) /'('", Color.Whitespace, Color.Structure);

         while (state.More)
         {
            if (state.CurrentSource.StartsWith(")"))
               break;

            if (getTerm().Failed(out var exception))
               return failedMatch<LambdaSymbol>(exception);

            if (infixParser.Scan(state).If(out _, out var isNotMatched, out exception))
            {
               if (getTerm().Failed(out exception))
                  return failedMatch<LambdaSymbol>(exception);
            }
            else if (isNotMatched)
               break;
            else
               return failedMatch<LambdaSymbol>(exception);
         }

         var parameterCount = unknownFieldCount.MaxOf(maxFieldCount) + (addOne ? 1 : 0);
         if (state.Scan("^ /')'", Color.Structure).If(out _, out var scanOriginal))
            return builder.ToExpression().FlatMap(expression => new LambdaSymbol(parameterCount, expression).Matched(),
               failedMatch<LambdaSymbol>);
         else
            return scanOriginal.Unmatched<LambdaSymbol>();
      }

      public static IMatched<IConstant> getConstant(ParseState state)
      {
         var builder = new ExpressionBuilder(ExpressionFlags.Standard);
         var parser = new ConstantsParser(builder);
         if (parser.Scan(state).If(out _, out var original))
            if (builder.ToExpression().If(out var expression, out var exception))
            {
               var symbol = expression.Symbols[0];
               if (symbol is IConstant c)
                  return c.Matched();
               else
                  return $"Expected constant, found {symbol}".FailedMatch<IConstant>();
            }
            else
               return failedMatch<IConstant>(exception);
         else
            return original.Unmatched<IConstant>();
      }

      public static BigInteger convert(string source, int baseValue, string possible)
      {
         source = source.Reverse();
         var accum = BigInteger.Zero;
         var bigBase = (BigInteger)baseValue;
         for (var exponent = 0; exponent < source.Length; exponent++)
         {
            var raised = BigInteger.Pow(bigBase, exponent);
            var index = possible.IndexOf(source[exponent]);
            accum += raised * index;
         }

         return accum;
      }

      public static IMatched<Unit> getNumber(ExpressionBuilder builder, string type, string source)
      {
         switch (type)
         {
            case "":
               if (TryParse(source, out var integer))
               {
                  builder.Add(new IntSymbol(integer));
                  return Unit.Matched();
               }
               else
                  return failedMatch<Unit>(unableToConvert(source, "Int"));
            case "L":
               if (BigInteger.TryParse(source, out var bigInteger))
               {
                  builder.Add(new LongSymbol(bigInteger));
                  return Unit.Matched();
               }
               else
                  return failedMatch<Unit>(unableToConvert(source, "Long"));
            case "i":
               if (TryParse(source, out integer))
               {
                  builder.Add(new ComplexSymbol(integer));
                  return Unit.Matched();
               }
               else
                  return failedMatch<Unit>(unableToConvert(source, "Complex"));
            case "f":
               if (double.TryParse(source, out var real))
               {
                  builder.Add(new FloatSymbol(real));
                  return Unit.Matched();
               }
               else
                  return failedMatch<Unit>(unableToConvert(source, "Float"));
            default:
               return failedMatch<Unit>(unableToConvert(source, "Int"));
         }
      }

      public static IMatched<Unit> getNumber(ExpressionBuilder builder, string type, BigInteger number)
      {
         switch (type)
         {
            case "":
               if (number < MinValue || number > MaxValue)
               {
                  builder.Add(new LongSymbol(number));
                  return Unit.Matched();
               }
               else
               {
                  builder.Add(new IntSymbol((int)number));
                  return Unit.Matched();
               }
            case "L":
               builder.Add(new LongSymbol(number));
               return Unit.Matched();
            case "i":
               builder.Add(new ComplexSymbol((double)number));
               return Unit.Matched();
            case "f":
               builder.Add(new FloatSymbol((double)number));
               return Unit.Matched();
            default:
               return failedMatch<Unit>(unableToConvert(number.ToString(), "Int"));
         }
      }

      public static IMatched<LambdaSymbol> getAnyLambda(ParseState state, Bits32<ExpressionFlags> flags)
      {
         var builder = new ExpressionBuilder(flags);
         return new AnyLambdaParser(builder).Scan(state).Map(u =>
         {
            if (builder.ToExpression().If(out var expression, out var exception))
               return ((LambdaSymbol)expression.Symbols[0]).Matched();
            else
               return failedMatch<LambdaSymbol>(exception);
         });
      }

      public static IMatched<IMaybe<LambdaSymbol>> getPossibleLambda(ParseState state, Bits32<ExpressionFlags> flags)
      {
         if (getAnyLambda(state, flags).If(out var lambdaSymbol, out var isNotMatched, out var exception))
            return lambdaSymbol.Some().Matched();
         else if (isNotMatched)
            return none<LambdaSymbol>().Matched();
         else
            return failedMatch<IMaybe<LambdaSymbol>>(exception);
      }

      public static IMatched<(Symbol, Expression, IMaybe<Expression>)> getInnerComprehension(ParseState state) =>
         from comparisand in getValue(state, ExpressionFlags.Comparisand)
         from scanned in state.Scan("^ /(|s|) /'<-'", Color.Whitespace, Color.Structure)
         from source in getExpression(state, ExpressionFlags.OmitIf | ExpressionFlags.OmitComprehension)
         from ifExp in getIf(state)
         select (comparisand, source, ifExp);

      public static IMatched<IMaybe<Expression>> getIf(ParseState state)
      {
         if (state.Scan("^ /(|s+|) /'if' /b", Color.Whitespace, Color.Keyword).If(out _, out var isNotMatched, out var exception))
            if (getExpression(state, ExpressionFlags.OmitIf | ExpressionFlags.OmitComprehension)
               .If(out var expression, out var original))
               return expression.Some().Matched();
            else
               return original.Unmatched<IMaybe<Expression>>();
         else if (isNotMatched)
            return none<Expression>().Matched();
         else
            return failedMatch<IMaybe<Expression>>(exception);
      }

      public static IMatched<IMaybe<Expression>> getAnd(ParseState state)
      {
         var builder = new ExpressionBuilder(ExpressionFlags.OmitIf);
         var parser = new IfAsAndParser(builder);
         if (parser.Scan(state).If(out _, out var isNotMatched, out var exception))
            if (builder.ToExpression().If(out var expression, out exception))
               return expression.Some().Matched();
            else
               return failedMatch<IMaybe<Expression>>(exception);
         else if (isNotMatched)
            return none<Expression>().Matched();
         else
            return failedMatch<IMaybe<Expression>>(exception);
      }

      public static void addMatchElse(If ifStatement)
      {
         var current = ifStatement;
         while (current.ElseIf.If(out var nextIf))
            current = nextIf;

         current.Else = new Block(new Return(new Expression(new ObjectSymbol(Unmatched.Value)))).Some();
      }

      public static IMatched<IMaybe<AndSymbol>> andExpression(ParseState state)
      {
         var builder = new ExpressionBuilder(ExpressionFlags.Standard);
         var andParser = new IfAsAndParser(builder);
         if (andParser.Scan(state).If(out _, out var isNotMatched, out var exception))
            return builder.ToExpression().FlatMap(e => ((AndSymbol)e.Symbols[0]).Some().Matched(), failedMatch<IMaybe<AndSymbol>>);
         else if (isNotMatched)
            return none<AndSymbol>().Matched();
         else
            return failedMatch<IMaybe<AndSymbol>>(exception);
      }

      public static IMatched<Block> getCaseStatementBlock(ParseState state)
      {
         if (state.Scan("^ /(|s|) /'=' -(> '=')", Color.Whitespace, Color.Structure).IsMatched)
            return getSingleLine(state, false);
         else
            return getBlock(state);
      }

      public static IMatched<Symbol> getOperator(ParseState state, string source, Bits32<ExpressionFlags> flags, bool whitespace)
      {
         var symbol = notMatched<Symbol>();
         switch (source)
         {
            case "+":
               symbol = new AddSymbol().Matched<Symbol>();
               break;
            case "-":
               symbol = new SubtractSymbol().Matched<Symbol>();
               break;
            case "*":
               symbol = new MultiplySymbol().Matched<Symbol>();
               break;
            case "/":
               if (whitespace)
                  symbol = new FloatDivideSymbol().Matched<Symbol>();
               else
                  symbol = new RationalSymbol().Matched<Symbol>();
               break;
            case "//":
               symbol = new IntDivideSymbol().Matched<Symbol>();
               break;
            case "/:":
               symbol = new SendBinaryMessageSymbol("foldl", Precedence.ChainedOperator).Matched<Symbol>();
               break;
            case @":\":
               symbol = new SendBinaryMessageSymbol("foldr", Precedence.ChainedOperator).Matched<Symbol>();
               break;
            case "%":
               symbol = new RemainderSymbol().Matched<Symbol>();
               break;
            case "%%":
               symbol = new RemainderZero().Matched<Symbol>();
               break;
            case "^":
               symbol = new RaiseSymbol().Matched<Symbol>();
               break;
            case "==":
               symbol = new EqualSymbol().Matched<Symbol>();
               break;
            case "!=":
               symbol = new NotEqualSymbol().Matched<Symbol>();
               break;
            case ">":
               symbol = new GreaterThanSymbol().Matched<Symbol>();
               break;
            case ">=":
               symbol = new GreaterThanEqualSymbol().Matched<Symbol>();
               break;
            case "<":
               symbol = new LessThanSymbol().Matched<Symbol>();
               break;
            case "<=":
               symbol = new LessThanEqualSymbol().Matched<Symbol>();
               break;
            case "++":
               symbol = new RangeSymbol(true).Matched<Symbol>();
               break;
            case "+-":
               symbol = new RangeSymbol(false).Matched<Symbol>();
               break;
            case "--":
               symbol = new RangeSymbol(true, true).Matched<Symbol>();
               break;
            case "::":
               symbol = new ConsSymbol().Matched<Symbol>();
               break;
            case "\\":
               symbol = new FormatSymbol().Matched<Symbol>();
               break;
            case ",":
               if (flags[ExpressionFlags.OmitComma])
                  return notMatched<Symbol>();
               else
               {
                  state.Scan("^ /(/s*)", Color.Whitespace);
                  symbol = new CommaSymbol().Matched<Symbol>();
               }

               break;
            case "~":
               symbol = new ConcatenationSymbol().Matched<Symbol>();
               break;
            case "<<":
            case ">>":
               symbol = new SendBinaryMessageSymbol(source, Precedence.Shift).Matched<Symbol>();
               break;
            case "=>":
               symbol = new KeyValueSymbol().Matched<Symbol>();
               break;
            case "|>":
               symbol = new PipelineSymbol().Matched<Symbol>();
               break;
            case "**":
               symbol = new OpenRangeSymbol().Matched<Symbol>();
               break;
            case "<>":
               symbol = new CompareSymbol().Matched<Symbol>();
               break;
         }

         return symbol;
      }
   }
}