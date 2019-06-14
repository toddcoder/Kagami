using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class SubexpressionParser : SymbolParser
	{
/*		public SubexpressionParser(ExpressionBuilder builder) : base(builder, "^ /')'", Color.Structure) { }

		public override string Pattern => "^ /(|s|) /'('";

		public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
		{
			state.BeginTransaction();
			state.Colorize(tokens, Color.Whitespace, Color.Structure);

			return Unit.Matched();
		}

		public override IMatched<Unit> Suffix(ParseState state, Expression expression)
		{
			builder.Add(new SubexpressionSymbol(expression));
			state.CommitTransaction();

			return Unit.Matched();
		}

		public override IMatched<Unit> OnFailure(ParseState state, Exception exception)
		{
			state.RollBackTransaction();
			state.BeginTransaction();
			if (getPartialLambda(state).Out(out var lambdaSymbol, out var original))
			{
				state.CommitTransaction();
				builder.Add(lambdaSymbol);

				return Unit.Matched();
			}
			else
			{
				state.RollBackTransaction();
				return original.Unmatched<Unit>();
			}
		}*/
		public SubexpressionParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /(|s|) /'('";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.BeginTransaction();
			state.Colorize(tokens, Color.Whitespace, Color.Structure);

			if (getExpression(state, builder.Flags).If(out var expression, out var anyException))
			{
				var monoTuple = false;
				if (state.Scan("^ /(|s|) (/',' /(|s|))? /')'", Color.Whitespace, Color.Structure, Color.Whitespace, Color.Structure)
					.If(out var scanned, out anyException))
				{
					if (scanned.Contains(","))
						monoTuple = true;

					builder.Add(new SubexpressionSymbol(expression, monoTuple));
				}
				else if (anyException.If(out _))
				{
					state.RollBackTransaction();
					state.BeginTransaction();
					if (getPartialLambda(state).Out(out var lambdaSymbol, out var original))
					{
						state.CommitTransaction();
						builder.Add(lambdaSymbol);

						return Unit.Matched();
					}
					else
					{
						state.RollBackTransaction();
						return original.Unmatched<Unit>();
					}
				}
				else
					return notMatched<Unit>();
			}
			else if (anyException.If(out var exception))
			{
				state.RollBackTransaction();
				return failedMatch<Unit>(exception);
			}
			else
				return notMatched<Unit>();

			return Unit.Matched();
		}
	}
}