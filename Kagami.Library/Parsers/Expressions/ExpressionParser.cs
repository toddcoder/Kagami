using Core.Monads;
using Core.Numbers;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using System.Linq;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class ExpressionParser : Parser
	{
		protected Bits32<ExpressionFlags> flags;
		protected ExpressionBuilder builder;
		protected PrefixParser prefixParser;
		protected ValuesParser valuesParser;
		protected InfixParser infixParser;
		protected PostfixParser postfixParser;
		protected ConjunctionParsers conjunctionParsers;
		protected int whateverCount;
		protected IMaybe<IPrefixCode> prefixCode;

		public ExpressionParser(Bits32<ExpressionFlags> flags) : base(false) => this.flags = flags;

		public Expression Expression { get; set; }

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens)
		{
			if (!state.More)
				return notMatched<Unit>();

			builder = new ExpressionBuilder(flags);
			prefixParser = new PrefixParser(builder);
			valuesParser = new ValuesParser(builder);
			prefixCode = none<IPrefixCode>();
			infixParser = new InfixParser(builder);
			postfixParser = new PostfixParser(builder);
			conjunctionParsers = new ConjunctionParsers(builder);
			whateverCount = 0;

			state.BeginPrefixCode();

			try
			{
				if (getTerm(state).If(out _, out var mbException))
				{
					while (state.More)
					{
						if (!flags[ExpressionFlags.OmitConjunction])
						{
							var conjunction = conjunctionParsers.Scan(state);
							if (conjunction.IsMatched)
								break;
							else if (conjunction.IsFailedMatch)
								return conjunction;
						}

						if (infixParser.Scan(state).If(out _, out mbException))
							if (getTerm(state).If(out _, out mbException)) { }
							else if (mbException.If(out var exception))
								return failedMatch<Unit>(exception);
							else
								break;
						else if (mbException.If(out var exception))
							return failedMatch<Unit>(exception);
						else
							break;
					}

					if (builder.ToExpression().If(out var expression, out var expException))
					{
						if (state.ImplicitState.If(out var implicitState))
						{
							if (getMessageWithLambda(implicitState.Symbol, implicitState.Message, implicitState.ParameterCount, expression)
								.If(out var newExpression, out expException))
							{
								Expression = newExpression;
								state.ImplicitState = none<ImplicitState>();
							}
							else
								return failedMatch<Unit>(expException);
						}
						else if (state.LeftZipExpression.If(out var leftTuple) && state.RightZipExpression.If(out var rightTuple))
						{
							var (leftFieldName, leftSymbol) = leftTuple;
							var (rightFieldName, rightSymbol) = rightTuple;
							if (!keep(leftFieldName) || !keep(rightFieldName))
							{
								Expression = expression;
								return Unit.Matched();
							}
							else if (getDualMessageWithLambda(leftFieldName, rightFieldName, leftSymbol, rightSymbol,
								"zip".Selector("<Collection>", "<Lambda>"),
								expression).If(out var newExpression, out expException))
							{
								Expression = newExpression;
								state.LeftZipExpression = none<(string, Symbol)>();
								state.RightZipExpression = none<(string, Symbol)>();
							}
							else
								return failedMatch<Unit>(expException);
						}
						else if (whateverCount > 0)
						{
							var lambda = new LambdaSymbol(whateverCount,
								new Block(new ExpressionStatement(expression, true)) { Index = expression.Index });
							Expression = new Expression(lambda);
						}
						else
							Expression = expression;

						return Unit.Matched();
					}
					else
						return failedMatch<Unit>(expException);
				}
				else if (mbException.If(out var exception))
					return failedMatch<Unit>(exception);
				else
					return "Invalid expression syntax".FailedMatch<Unit>();
			}
			finally
			{
				state.EndPrefixCode();
			}
		}

		bool keep(string fieldName)
		{
			var exp = builder.Ordered.ToArray();
			if (exp.Length != 1)
				return true;
			else
				return !(exp[0] is FieldSymbol fieldSymbol) || fieldSymbol.FieldName != fieldName;
		}

		static IResult<Expression> getMessageWithLambda(Symbol symbol, Selector selector, int parameterCount, Expression expression)
		{
			var parameters = new Parameters(Enumerable.Range(0, parameterCount).Select(i => $"__${i}").ToArray());
			var lambdaSymbol = new LambdaSymbol(parameters, new Block(new Return(expression, none<TypeConstraint>())));
			var sendMessage = new SendMessageSymbol(selector, Precedence.SendMessage, lambdaSymbol.Some());

			var builder = new ExpressionBuilder(ExpressionFlags.Standard);
			builder.Add(symbol);
			builder.Add(sendMessage);

			return builder.ToExpression();
		}

		static IResult<Expression> getMessage2WithLambda(string leftName, string rightName, Symbol symbol, Selector selector,
			Expression expression)
		{
			var leftParameter = Parameter.New(false, leftName);
			var rightParameter = Parameter.New(false, rightName);
			var parameters = new Parameters(leftParameter, rightParameter);
			var lambdaSymbol = new LambdaSymbol(parameters, new Block(new Return(expression, none<TypeConstraint>())));
			var sendMessage = new SendMessageSymbol(selector, Precedence.SendMessage, lambdaSymbol.Some());

			var builder = new ExpressionBuilder(ExpressionFlags.Standard);
			builder.Add(symbol);
			builder.Add(sendMessage);

			return builder.ToExpression();
		}

		static IResult<Expression> getDualMessageWithLambda(string leftName, string rightName, Symbol leftSymbol, Symbol rightSymbol,
			Selector selector, Expression expression)
		{
			var leftParameter = Parameter.New(false, leftName);
			var rightParameter = Parameter.New(false, rightName);
			var parameters = new Parameters(leftParameter, rightParameter);
			var lambdaSymbol = new LambdaSymbol(parameters, new Block(new Return(expression, none<TypeConstraint>())));
			var sendMessage = new SendMessageSymbol(selector, Precedence.SendMessage, lambdaSymbol.Some(),
				new Expression(rightSymbol));

			var builder = new ExpressionBuilder(ExpressionFlags.Standard);
			builder.Add(leftSymbol);
			builder.Add(sendMessage);

			return builder.ToExpression();
		}

		protected static IMatched<Unit> getOutfixOperator(ParseState state, Parser parser)
		{
			var matched = parser.Scan(state);
			if (matched.IsFailedMatch)
				return matched;
			else
			{
				while (matched.IsMatched)
				{
					matched = parser.Scan(state);
					if (matched.IsFailedMatch)
						return matched;
				}

				if (matched.IsFailedMatch)
					return matched;
				else
					return notMatched<Unit>();
			}
		}

		protected IMatched<Unit> getValue(ParseState state)
		{
			if (valuesParser.Scan(state).Out(out _, out var original))
			{
				if (builder.LastSymbol.If(out var lastSymbol) && lastSymbol is WhateverSymbol whatever)
					whatever.Count = whateverCount++;
				return Unit.Matched();
			}
			else if (original.IsFailedMatch)
				return valuesParser.Scan(state);
			else
				return "Invalid expression syntax".FailedMatch<Unit>();
		}

		protected IMatched<Unit> getTerm(ParseState state)
		{
			if ((getOutfixOperator(state, prefixParser).Out(out _, out var original) || original.IsNotMatched) &&
				getValue(state).Out(out _, out original) &&
				(getOutfixOperator(state, postfixParser).Out(out _, out original) || original.IsNotMatched))
				return Unit.Matched();
			else
				return original;
		}
	}
}