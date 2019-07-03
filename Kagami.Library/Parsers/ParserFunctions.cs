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
using Return = Kagami.Library.Nodes.Statements.Return;

namespace Kagami.Library.Parsers
{
	public static class ParserFunctions
	{
		public const string REGEX_FIELD = "['A-Za-z_'] ['A-Za-z_0-9']*";
		public const string REGEX_INVOKABLE = "['A-Za-z_'] ['A-Za-z_0-9']*";
		public const string REGEX_CLASS = "['A-Z'] ['A-Za-z_0-9']*";
		public const string REGEX_CLASS_GETTING = REGEX_CLASS + "('.' " + REGEX_CLASS + ")?";
		public const string REGEX_ASSIGN_OPS = "'+' | '-' | '*' | '////' | '//' | '^'";
		public const string REGEX_FUNCTION_NAME = "((" + REGEX_INVOKABLE + ") | (['~`!@#$%^&*+=|\\;<>//?-']+) | '[]') '='?";
		public const string REGEX_EOL = "/r /n | /r | /n";
		public const string REGEX_ANTICIPATE_END = "(> (" + REGEX_EOL + ") | $)";
		public const string REGEX_OPERATORS = "['-+*//\\%<=>!.~|?#@&^,;.:']";
		public const string REGEX_ITERATOR_FUNCTIONS = "'sort' | 'foldl' | 'foldr' | 'reducel' | 'reducer' | " +
			"'count' | 'map' | 'flatMap' | 'bind' | 'if' | 'ifNot' | 'index' | 'indexes' | 'min' | 'max' | 'first' | " +
			"'last' | 'split' | 'one' | 'none' | 'any' | 'all' | 'span' | 'groupBy' | 'each' | 'while' | 'until' | 'z' | 'zip' | 'x' | 'cross' | 'acc' | 'accr' |" +
			" 'seq' | 'takeWhile' | 'takeUntil' | 'skipWhile' | 'skipUntil'";
		public const string REGEX_LIST_LEFT = "⌈";
		public const string REGEX_LIST_RIGHT = "⌉";

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

		public static IMatched<Expression> getCompoundComparisands(ParseState state, string fieldName)
		{
			var flags = ExpressionFlags.Comparisand | ExpressionFlags.OmitAnd | ExpressionFlags.OmitIf;
			var builder = new ExpressionBuilder(flags);

			if (getExpression(state, flags).Out(out var comparisand, out var original))
			{
				builder.Add(new FieldSymbol(fieldName));
				builder.Add(comparisand);
				builder.Add(new MatchSymbol());
				if (state.Scan("^ /(|s|) /'&'", Color.Whitespace, Color.OpenParenthesis).If(out _, out var anyException))
				{
					return getCompoundComparisands(state, fieldName).Map(nextExpression =>
					{
						builder.Add(new AndSymbol(nextExpression));
						return builder.ToExpression().Match();
					});
				}
				else if (anyException.If(out var exception))
				{
					return failedMatch<Expression>(exception);
				}
				else
				{
					return builder.ToExpression().Match();
				}
			}
			else
			{
				return original.Unmatched<Expression>();
			}
		}

		public static IMatched<InternalList> getInternalList(ParseState state)
		{
			var builder = new ExpressionBuilder(ExpressionFlags.Standard);
			var constantsParser = new ConstantsParser(builder);

			while (state.More)
			{
				var result = constantsParser.Scan(state);
				if (result.IsMatched)
				{
					if (state.Scan("^ /(|s|) /','", Color.Whitespace, Color.Operator).IsMatched) { }
					else if (result.Failed(out var exception))
					{
						return failedMatch<InternalList>(exception);
					}
					else
					{
						break;
					}
				}
				else if (result.Failed(out var exception))
				{
					return failedMatch<InternalList>(exception);
				}
				else
				{
					break;
				}
			}

			if (builder.ToExpression().If(out var expression, out var expException))
			{
				var symbols = expression.Symbols;
				var list = new List<IObject>();
				foreach (var symbol in symbols)
				{
					if (symbol is IConstant c)
					{
						list.Add(c.Object);
					}
					else
					{
						return $"Expected constant, found {symbol}".FailedMatch<InternalList>();
					}
				}

				return new InternalList(list).Matched();
			}
			else
			{
				return failedMatch<InternalList>(expException);
			}
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
				case "^":
					return new Raise().Matched<Operation>();
				default:
					return $"Didn't recognize operator {source}".FailedMatch<Operation>();
			}
		}

		public static IMatched<Block> getBlock(ParseState state, IMaybe<TypeConstraint> typeConstraint)
		{
			if (state.Advance().If(out _, out var anyException))
			{
				var statementsParser = new StatementsParser();
				state.PushStatements();
				while (state.More)
				{
					if (statementsParser.Scan(state).If(out _, out var mbStatementException)) { }
					else if (mbStatementException.If(out var statementException))
					{
						return failedMatch<Block>(statementException);
					}
					else
					{
						break;
					}
				}

				return state.PopStatements().FlatMap(statements =>
				{
					state.Regress();
					return new Block(statements, typeConstraint).Matched();
				}, failedMatch<Block>);
			}
			else if (anyException.If(out var exception))
			{
				return failedMatch<Block>(exception);
			}
			else
			{
				return failedMatch<Block>(badIndentation());
			}
		}

		public static IMatched<Block> getBlock(ParseState state) => getBlock(state, none<TypeConstraint>());

		public static IMatched<Block> getSingleLine(ParseState state, IMaybe<TypeConstraint> typeConstraint,
			bool returnExpression = true)
		{
			var statementsParser = new StatementsParser(true) { ReturnExpression = returnExpression, TypeConstraint = typeConstraint };
			state.PushStatements();
			if (statementsParser.Scan(state).If(out _, out var anyException))
			{
				if (state.PopStatements().If(out var statements, out var exception))
				{
					return new Block(statements, typeConstraint).Matched();
				}
				else
				{
					return failedMatch<Block>(exception);
				}
			}
			else if (anyException.If(out var exception))
			{
				return failedMatch<Block>(exception);
			}
			else
			{
				state.PopStatements();
				return notMatched<Block>();
			}
		}

		public static IMatched<Block> getSingleLine(ParseState state, bool returnExpression = true)
		{
			return getSingleLine(state, none<TypeConstraint>(), returnExpression);
		}

		public static IMatched<Symbol> getValue(ParseState state, Bits32<ExpressionFlags> flags)
		{
			var builder = new ExpressionBuilder(flags);
			var parser = new ValuesParser(builder);
			return parser.Scan(state).Map(u => builder.Ordered.ToArray()[0]);
		}

		public static IMatched<Parameters> getParameters(ParseState state)
		{
			if (state.Scan("^ /[')]']", Color.CloseParenthesis).If(out _, out var anyException))
			{
				return new Parameters().Matched();
			}
			else if (anyException.If(out var exception))
			{
				return failedMatch<Parameters>(exception);
			}

			var parameters = new List<Parameter>();
			var defaultRequired = false;
			var continuing = true;

			while (state.More && continuing)
			{
				if (getParameter(state, defaultRequired).Out(out var parameter, out var originalParameter))
				{
					if (parameter.DefaultValue.IsSome)
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
					return originalParameter.UnmatchedOnly<Parameters>();
				}

				if (state.Scan("^ /(/s*) /[',)']", Color.Whitespace, Color.CloseParenthesis).Out(out var next, out var nextOriginal))
				{
					if (next.EndsWith(")"))
					{
						return new Parameters(parameters.ToArray()).Matched();
					}
				}
				else
				{
					return nextOriginal.UnmatchedOnly<Parameters>();
				}

				if (!continuing)
				{
					return "There can be no parameters after a variadic parameter".FailedMatch<Parameters>();
				}
			}

			return failedMatch<Parameters>(openParameters());
		}

		public static IMatched<Expression[]> getArguments(ParseState state, Bits32<ExpressionFlags> flags)
		{
			if (state.Scan("^ /[')]}']", Color.CloseParenthesis).If(out _, out var anyScanException))
			{
				return new Expression[0].Matched();
			}
			else if (anyScanException.If(out var scanException))
			{
				return failedMatch<Expression[]>(scanException);
			}

			var arguments = new List<Expression>();
			var scanning = true;

			while (state.More && scanning)
			{
				if (getExpression(state, flags | ExpressionFlags.OmitComma | ExpressionFlags.InArgument)
					.If(out var expression, out var anyException))
				{
					arguments.Add(expression);
					if (state.Scan("^ /(/s*) /[',)]}']", Color.Whitespace, Color.CloseParenthesis).Out(out var next, out var original))
					{
						if (next.EndsWith(")") || next.EndsWith("]") || next.EndsWith("}"))
						{
							return arguments.ToArray().Matched();
						}
					}
					else
					{
						return original.UnmatchedOnly<Expression[]>();
					}
				}
				else if (anyException.If(out var exception))
				{
					return failedMatch<Expression[]>(exception);
				}
				else
				{
					scanning = false;
				}
			}

			return failedMatch<Expression[]>(openArguments());
		}

		public static IMatched<(Expression[], IMaybe<LambdaSymbol>)> getArgumentsPlusLambda(ParseState state,
			Bits32<ExpressionFlags> flags)
		{
			return from arguments in getArguments(state, flags)
				from lambda in getPossibleLambda(state, flags)
				select (arguments, lambda);
		}

		public static IMatched<IObject> getComparisand(ParseState state)
		{
			if (getExpression(state, ExpressionFlags.Comparisand | ExpressionFlags.OmitComma).Out(out var expression, out var original))
			{
				if (expression.Symbols[0] is IConstant constant)
				{
					return constant.Object.Matched();
				}
				else
				{
					return failedMatch<IObject>(constantRequired(expression));
				}
			}
			else
			{
				return original.Unmatched<IObject>();
			}
		}

		public static IMatched<IObject[]> getComparisandList(ParseState state)
		{
			if (state.Scan("^ /[')]']", Color.Structure).If(out _, out var mbScanException))
			{
				return new IObject[0].Matched();
			}
			else if (mbScanException.If(out var scanException))
			{
				return failedMatch<IObject[]>(scanException);
			}

			var arguments = new List<IObject>();
			var scanning = true;

			while (state.More && scanning)
			{
				if (getComparisand(state).If(out var comparisand, out var anyException))
				{
					arguments.Add(comparisand);
					if (state.Scan("^ /(/s*) /[',)]']", Color.Whitespace, Color.Structure).Out(out var next, out var nextOriginal))
					{
						if (next.EndsWith(")") || next.EndsWith("]"))
						{
							return arguments.ToArray().Matched();
						}
					}
					else
					{
						return nextOriginal.UnmatchedOnly<IObject[]>();
					}
				}
				else if (anyException.If(out var exception))
				{
					return failedMatch<IObject[]>(exception);
				}
				else
				{
					scanning = false;
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

		static IMatched<string> parseLabel(ParseState state)
		{
			return state.Scan($"^ (/(/s*) /({REGEX_FIELD}) /':')?", Color.Whitespace, Color.Label, Color.Structure)
				.Map(s => s.KeepUntil(":").Trim());
		}

		static IMatched<bool> parseCapturing(ParseState state)
		{
			return state.Scan("^ /(/s* '+')?", Color.Structure).Map(s => s.IsNotEmpty());
		}

		static IMatched<string> parseParameterName(ParseState state)
		{
			return state.Scan($"^ /(/s* {REGEX_FIELD}) /b", Color.Identifier).Map(s => s.Trim());
		}

		public static IMatched<IMaybe<TypeConstraint>> parseTypeConstraint(ParseState state)
		{
			if (state.Scan($"^ /(|s|) /({REGEX_CLASS}) -(> '(') /b", Color.Whitespace, Color.Class)
				.If(out var className, out var anyException))
			{
				className = className.TrimStart();
				if (Module.Global.Class(className).If(out var baseClass))
				{
					return new TypeConstraint(new[] { baseClass }).Some().Matched();
				}
				else if (Module.Global.Forwarded(className))
				{
					return new TypeConstraint(new[] { new ForwardedClass(className) }).Some().Matched();
				}
				else
				{
					return failedMatch<IMaybe<TypeConstraint>>(classNotFound(className));
				}
			}
			else if (anyException.If(out var exception))
			{
				return failedMatch<IMaybe<TypeConstraint>>(exception);
			}
			else
			{
				var builder = new ExpressionBuilder(ExpressionFlags.Standard);
				var typeConstraintParser = new TypeConstraintParser(builder);
				if (typeConstraintParser.Scan(state).If(out _, out anyException))
				{
					var typeConstraint = (TypeConstraint)((IConstant)builder.Ordered.ToArray()[0]).Object;
					return typeConstraint.Some().Matched();
				}
				else if (anyException.If(out exception))
				{
					return failedMatch<IMaybe<TypeConstraint>>(exception);
				}
				else
				{
					return none<TypeConstraint>().Matched();
				}
			}
		}

		static IMatched<bool> parseVaraidic(ParseState state)
		{
			if (state.Scan("^ /(|s|) /'...'", Color.Whitespace, Color.Structure).If(out _, out var anyException))
			{
				return true.Matched();
			}
			else if (anyException.If(out var exception))
			{
				return failedMatch<bool>(exception);
			}
			else
			{
				return false.Matched();
			}
		}

		static IMatched<IMaybe<IInvokable>> parseDefaultValue(ParseState state, bool defaultRequired)
		{
			var result = state.Scan("^ /(/s* '=') -(> '=')", Color.Structure);
			if (result.If(out _, out var anyException))
			{
				return getExpression(state, ExpressionFlags.OmitComma).Map(e =>
				{
					var symbol = new InvokableExpressionSymbol(e);
					state.AddSymbol(symbol);
					return symbol.Invokable.Some().Matched();
				});
			}
			else if (anyException.If(out var exception))
			{
				return failedMatch<IMaybe<IInvokable>>(exception);
			}
			else if (defaultRequired)
			{
				return "default required".FailedMatch<IMaybe<IInvokable>>();
			}
			else
			{
				return none<IInvokable>().Matched();
			}
		}

		static IMatched<Parameter> getParameter(ParseState state, bool defaultRequired) =>
			from reference in parseReference(state)
			from mutable in parseMutable(state)
			from label in parseLabel(state)
			from capturing in parseCapturing(state)
			from name in parseParameterName(state)
			from typeConstraint in parseTypeConstraint(state)
			from variadic in parseVaraidic(state)
			from defaultValue in parseDefaultValue(state, defaultRequired)
			select new Parameter(mutable, label, name, defaultValue, typeConstraint, reference, capturing) { Variadic = variadic };

		public static IMatched<Block> getAnyBlock(ParseState state)
		{
			if (parseTypeConstraint(state).If(out var anyTypeConstraint, out var anyException))
			{
				state.SetReturnType(anyTypeConstraint);
				if (state.Scan("^ /(|s|) /'=' /(|s|)", Color.Whitespace, Color.Structure, Color.Whitespace).IsMatched)
				{
					return getSingleLine(state, anyTypeConstraint);
				}
				else
				{
					return getBlock(state, anyTypeConstraint);
				}
			}
			else if (anyException.If(out var exception))
			{
				return failedMatch<Block>(exception);
			}
			else
			{
				return notMatched<Block>();
			}
		}

		public static IResult<If> getIf(string parameterName, Symbol comparisand, IMaybe<Expression> and, Block block)
		{
			var builder = new ExpressionBuilder(ExpressionFlags.Standard);
			builder.Add(new FieldSymbol(parameterName));
			builder.Add(new SendMessageSymbol("match", Precedence.SendMessage, new Expression(comparisand)));
			if (and.If(out var andExpression))
			{
				builder.Add(andExpression);
			}

			return builder.ToExpression().Map(expression => new If(expression, block));
		}

		public static IMatched<LambdaSymbol> getPartialLambda(ParseState state)
		{
			if (!state.More)
			{
				return notMatched<LambdaSymbol>();
			}

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
				if (valuesParser.Scan(state).Out(out _, out var original))
				{
					return Unit.Matched();
				}
				else if (original.IsFailedMatch)
				{
					return original;
				}

				if (unknownFieldParser.Scan(state).Out(out _, out original))
				{
					maxFieldCount = unknownFieldParser.Index.MaxOf(maxFieldCount);
					addOne = true;
					return Unit.Matched();
				}
				else if (original.IsFailedMatch)
				{
					return original;
				}

				builder.Add(new FieldSymbol($"__${unknownFieldCount++}"));
				return Unit.Matched();
			}

			IMatched<Unit> getTerm()
			{
				if (getValue().Out(out _, out var original)) { }
				else if (original.IsFailedMatch)
				{
					return original;
				}

				while (state.More)
				{
					if (postfixOperatorsParser.Scan(state).If(out _, out var anyException)) { }
					else if (anyException.If(out var exception))
					{
						return failedMatch<Unit>(exception);
					}
					else
					{
						break;
					}
				}

				return Unit.Matched();
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

					if (getTerm().Failed(out var exception))
					{
						return failedMatch<LambdaSymbol>(exception);
					}

					if (infixParser.Scan(state).If(out _, out var anyException))
					{
						if (getTerm().Failed(out exception))
						{
							return failedMatch<LambdaSymbol>(exception);
						}
					}
					else if (anyException.If(out exception))
					{
						return failedMatch<LambdaSymbol>(exception);
					}
					else
					{
						break;
					}
				}

				var parameterCount = unknownFieldCount.MaxOf(maxFieldCount) + (addOne ? 1 : 0);
				if (state.Scan("^ /')'", Color.CloseParenthesis).Out(out _, out var scanOriginal))
				{
					return builder.ToExpression().FlatMap(expression => new LambdaSymbol(parameterCount, expression).Matched(),
						failedMatch<LambdaSymbol>);
				}
				else
				{
					return scanOriginal.Unmatched<LambdaSymbol>();
				}
			}
			finally
			{
				state.EndPrefixCode();
				state.EndImplicitState();
			}
		}

		public static IMatched<IConstant> getConstant(ParseState state)
		{
			var builder = new ExpressionBuilder(ExpressionFlags.Standard);
			var parser = new ConstantsParser(builder);
			if (parser.Scan(state).Out(out _, out var original))
			{
				if (builder.ToExpression().If(out var expression, out var exception))
				{
					var symbol = expression.Symbols[0];
					if (symbol is IConstant c)
					{
						return c.Matched();
					}
					else
					{
						return $"Expected constant, found {symbol}".FailedMatch<IConstant>();
					}
				}
				else
				{
					return failedMatch<IConstant>(exception);
				}
			}
			else
			{
				return original.Unmatched<IConstant>();
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
					{
						return failedMatch<Unit>(unableToConvert(source, "Int"));
					}

				case "L":
					if (BigInteger.TryParse(source, out var bigInteger))
					{
						builder.Add(new LongSymbol(bigInteger));
						return Unit.Matched();
					}
					else
					{
						return failedMatch<Unit>(unableToConvert(source, "Long"));
					}

				case "i":
					if (TryParse(source, out integer))
					{
						builder.Add(new ComplexSymbol(integer));
						return Unit.Matched();
					}
					else
					{
						return failedMatch<Unit>(unableToConvert(source, "Complex"));
					}

				case "f":
					if (double.TryParse(source, out var real))
					{
						builder.Add(new FloatSymbol(real));
						return Unit.Matched();
					}
					else
					{
						return failedMatch<Unit>(unableToConvert(source, "Float"));
					}

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
				{
					return ((LambdaSymbol)expression.Symbols[0]).Matched();
				}
				else
				{
					return failedMatch<LambdaSymbol>(exception);
				}
			});
		}

		public static IMatched<IMaybe<LambdaSymbol>> getPossibleLambda(ParseState state, Bits32<ExpressionFlags> flags)
		{
			if (state.CurrentSource.StartsWith("("))
			{
				return none<LambdaSymbol>().Matched();
			}
			else if (getAnyLambda(state, flags).If(out var lambdaSymbol, out var anyException))
			{
				return lambdaSymbol.Some().Matched();
			}
			else if (anyException.If(out var exception))
			{
				return failedMatch<IMaybe<LambdaSymbol>>(exception);
			}
			else
			{
				return none<LambdaSymbol>().Matched();
			}
		}

		public static IMatched<(Symbol, Expression, IMaybe<Expression>)> getInnerComprehension(ParseState state) =>
			from comparisand in getValue(state, ExpressionFlags.Comparisand)
			from scanned in state.Scan("^ /(|s|) /':='", Color.Whitespace, Color.Structure)
			from source in getExpression(state, ExpressionFlags.OmitIf | ExpressionFlags.OmitComprehension)
			from ifExp in getIf(state)
			select (comparisand, source, ifExp);

		public static IMatched<IMaybe<Expression>> getIf(ParseState state)
		{
			if (state.Scan("^ /(|s+|) /'if' /b", Color.Whitespace, Color.Keyword).If(out _, out var anyException))
			{
				if (getExpression(state, ExpressionFlags.OmitIf | ExpressionFlags.OmitComprehension)
					.Out(out var expression, out var original))
				{
					return expression.Some().Matched();
				}
				else
				{
					return original.Unmatched<IMaybe<Expression>>();
				}
			}
			else if (anyException.If(out var exception))
			{
				return failedMatch<IMaybe<Expression>>(exception);
			}
			else
			{
				return none<Expression>().Matched();
			}
		}

		public static IMatched<IMaybe<Expression>> getAnd(ParseState state)
		{
			var builder = new ExpressionBuilder(ExpressionFlags.OmitIf);
			var parser = new IfAsAndParser(builder);
			if (parser.Scan(state).If(out _, out var anyException))
			{
				if (builder.ToExpression().If(out var expression, out var exception))
				{
					return expression.Some().Matched();
				}
				else
				{
					return failedMatch<IMaybe<Expression>>(exception);
				}
			}
			else if (anyException.If(out var exception))
			{
				return failedMatch<IMaybe<Expression>>(exception);
			}
			else
			{
				return none<Expression>().Matched();
			}
		}

		public static void addMatchElse(If ifStatement)
		{
			var current = ifStatement;
			while (current.ElseIf.If(out var nextIf))
			{
				current = nextIf;
			}

			current.Else = new Block(new Return(new Expression(new ObjectSymbol(Unmatched.Value)), none<TypeConstraint>())).Some();
		}

		public static IMatched<IMaybe<AndSymbol>> andExpression(ParseState state)
		{
			var builder = new ExpressionBuilder(ExpressionFlags.Standard);
			var andParser = new IfAsAndParser(builder);
			if (andParser.Scan(state).If(out _, out var anyException))
			{
				return builder.ToExpression().FlatMap(e => ((AndSymbol)e.Symbols[0]).Some().Matched(), failedMatch<IMaybe<AndSymbol>>);
			}
			else if (anyException.If(out var exception))
			{
				return failedMatch<IMaybe<AndSymbol>>(exception);
			}
			else
			{
				return none<AndSymbol>().Matched();
			}
		}

		public static IMatched<Block> getCaseStatementBlock(ParseState state)
		{
			if (state.Scan("^ /(|s|) /'=' -(> '=')", Color.Whitespace, Color.Structure).IsMatched)
			{
				return getSingleLine(state, false);
			}
			else
			{
				return getBlock(state);
			}
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
					{
						symbol = new FloatDivideSymbol().Matched<Symbol>();
					}
					else
					{
						symbol = new RationalSymbol().Matched<Symbol>();
					}

					break;
				case "//":
					symbol = new IntDivideSymbol().Matched<Symbol>();
					break;
				case "<!":
					symbol = new SendBinaryMessageSymbol("foldl", Precedence.ChainedOperator).Matched<Symbol>();
					break;
				case "<%":
					symbol = new SendBinaryMessageSymbol("sort".Selector("<Lambda>"), Precedence.ChainedOperator).Matched<Symbol>();
					break;
				case "!>":
					symbol = new SendBinaryMessageSymbol("foldr", Precedence.ChainedOperator).Matched<Symbol>();
					break;
				case "%":
					symbol = new RemainderSymbol().Matched<Symbol>();
					break;
				case "%%":
					symbol = new RemainderZeroSymbol().Matched<Symbol>();
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
					if (state.PrefixCode.If(out var prefixCode))
					{
						prefixCode.Prefix();
						symbol = new SpecialLessThanSymbol().Matched<Symbol>();
						state.PrefixCode = none<IPrefixCode>();
					}
					else
					{
						var lessThanSymbol = new LessThanSymbol();
						symbol = lessThanSymbol.Matched<Symbol>();
						state.PrefixCode = lessThanSymbol.Some<IPrefixCode>();
					}

					break;
				case "<=":
					if (state.PrefixCode.If(out prefixCode))
					{
						prefixCode.Prefix();
						symbol = new SpecialLessThanEqualSymbol().Matched<Symbol>();
						state.PrefixCode = none<IPrefixCode>();
					}
					else
					{
						var lessThanEqualSymbol = new LessThanEqualSymbol();
						symbol = lessThanEqualSymbol.Matched<Symbol>();
						state.PrefixCode = lessThanEqualSymbol.Some<IPrefixCode>();
					}

					break;
				case "::":
					symbol = new ConsSymbol().Matched<Symbol>();
					break;
				case "\\":
					symbol = new FormatSymbol().Matched<Symbol>();
					break;
				case ",":
					if (flags[ExpressionFlags.OmitComma])
					{
						return notMatched<Symbol>();
					}
					else
					{
						state.Scan("^ /(/s*)", Color.Whitespace);
						symbol = new CommaSymbol().Matched<Symbol>();
					}

					break;
				case "~":
					if (flags[ExpressionFlags.OmitConcatenate])
					{
						return notMatched<Symbol>();
					}
					else
					{
						symbol = new ConcatenationSymbol().Matched<Symbol>();
					}

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
				case "|=":
					symbol = new MatchSymbol().Matched<Symbol>();
					break;
				case "~~":
					symbol = new SendBinaryMessageSymbol("matches(_<String>)", Precedence.Boolean, true).Matched<Symbol>();
					break;
				case "!~":
					symbol = new SendBinaryMessageSymbol("notMatches(_<String>)", Precedence.Boolean, true).Matched<Symbol>();
					break;
				case ":-":
					symbol = new BindSymbol().Matched<Symbol>();
					break;
				case "##":
					symbol = new SendBinaryMessageSymbol("defaultTo(_)", Precedence.SendMessage).Matched<Symbol>();
					break;
			}

			return symbol;
		}

		public static IMatched<Expression> getTerm(ParseState state, ExpressionFlags flags)
		{
			var builder = new ExpressionBuilder(flags);
			var prefixParser = new PrefixParser(builder);
			var valuesParser = new ValuesParser(builder);
			var postfixParser = new PostfixParser(builder);

			while (state.More)
			{
				if (prefixParser.Scan(state).If(out _, out var anyPrefixException)) { }
				else if (anyPrefixException.If(out var prefixException))
				{
					return failedMatch<Expression>(prefixException);
				}
				else
				{
					break;
				}
			}

			if (valuesParser.Scan(state).If(out _, out var anyValueException)) { }
			else if (anyValueException.If(out var valueException))
			{
				return failedMatch<Expression>(valueException);
			}
			else
			{
				return failedMatch<Expression>(invalidSyntax());
			}

			while (state.More)
			{
				if (postfixParser.Scan(state).If(out _, out var anyPostException)) { }
				else if (anyPostException.If(out var postException))
				{
					return failedMatch<Expression>(postException);
				}
				else
				{
					break;
				}
			}

			if (builder.ToExpression().If(out var expression, out var exception))
			{
				return expression.Matched();
			}
			else
			{
				return failedMatch<Expression>(exception);
			}
		}

		public static IMatched<Block> getLambdaBlock(bool isExpression, ParseState state, Bits32<ExpressionFlags> flags,
			IMaybe<TypeConstraint> typeConstraint)
		{
			if (isExpression)
			{
				if (getExpression(state, flags).Out(out var expression, out var exOriginal))
				{
					return new Block(new ExpressionStatement(expression, true, typeConstraint), typeConstraint) { Index = state.Index }
						.Matched();
				}
				else
				{
					return exOriginal.Unmatched<Block>();
				}
			}
			else
			{
				return getBlock(state, typeConstraint);
			}
		}

		public static IMatched<(int, int, IMaybe<Expression>, IMaybe<Expression>)> getSkipTakeItem(ParseState state)
		{
			var parser = new SkipTakeItemParser();
			return parser.Scan(state).Map(u => (parser.Skip, parser.Take, parser.Prefix, parser.Suffix));
		}

		public static IMatched<SkipTakeItem[]> getSkipTakeItems(ParseState state)
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
					.If(out _, out var anyException)) { }
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
}