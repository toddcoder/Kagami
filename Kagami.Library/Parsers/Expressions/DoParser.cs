using System.Collections.Generic;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.RegularExpressions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class DoParser : SymbolParser
	{
		class BoundItemParser : EndingInExpressionParser
		{
			string fieldName;

			public BoundItemParser(ExpressionBuilder builder) : base(builder) => fieldName = "";

			public override string Pattern => $"^ /({REGEX_FIELD}) /(|s|) /'<-'";

			public (string, Expression) NameExpression { get; set; }

			public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
			{
				fieldName = tokens[1].Text;
				state.Colorize(tokens, Color.Identifier, Color.Whitespace, Color.Structure);

				return Unit.Matched();
			}

			public override IMatched<Unit> Suffix(ParseState state, Expression expression)
			{
				NameExpression = (fieldName, expression);
				return Unit.Matched();
			}
		}

		public DoParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => $"^ /(|s|) /'do' {REGEX_ANTICIPATE_END}";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Keyword);
			var innerBuilder = new ExpressionBuilder(builder.Flags);
			var boundItemParser = new BoundItemParser(innerBuilder);
			var stack = new Stack<(string, Expression)>();

			if (state.Advance().Out(out _, out var original))
			{
				while (state.More)
				{
					var result =
						from tabs in state.Scan($"^ /({state.Indentation.FriendlyString()})", Color.Whitespace)
						from unit in boundItemParser.Scan(state)
						select boundItemParser.NameExpression;
					if (result.If(out var nameExpression, out var mbException))
					{
						stack.Push(nameExpression);
						state.SkipEndOfLine();
					}
					else if (mbException.If(out var exception))
						return failedMatch<Unit>(exception);
					else
						break;
				}

				var lambdaResult = getExpression(state, builder.Flags);

				state.Regress();

				if (lambdaResult.If(out var lambdaExpression, out var mbLambdaException))
				{
					var (parameterName, targetExpression) = stack.Pop();
					if (getSymbol(targetExpression, parameterName, lambdaExpression, stack).If(out var symbol, out var exception))
					{
						builder.Add(symbol);
						return Unit.Matched();
					}
					else
						return failedMatch<Unit>(exception);
				}
				else if (mbLambdaException.If(out var exception))
					return failedMatch<Unit>(exception);
				else
					return "Missing 'gather' expression".FailedMatch<Unit>();
			}
			else
				return original;
		}

		static IResult<Symbol> getSymbol(Expression targetExpression, string parameterName, Expression lambdaExpression,
			Stack<(string, Expression)> stack)
		{
			var block = new Block(lambdaExpression);
			var lambda = new LambdaSymbol(new Parameters(parameterName), block);
			var builder = new ExpressionBuilder(ExpressionFlags.Standard);
			builder.Add(targetExpression);
			builder.Add(new SendMessageSymbol("bind(_<Lambda>)", Precedence.ChainedOperator, lambda.Some()));
			if (builder.ToExpression().If(out var expression, out var exception))
			{
				if (stack.Count == 0)
					return new SubexpressionSymbol(expression).Success<Symbol>();
				else
				{
					var (nextName, nextExpression) = stack.Pop();
					return getSymbol(nextExpression, nextName, expression, stack);
				}
			}
			else
				return failure<Symbol>(exception);
		}
	}
}