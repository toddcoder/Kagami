using System;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class SubexpressionParser : ExpressionInMiddleParser
	{
		public SubexpressionParser(ExpressionBuilder builder) : base(builder, "^ /')'", Color.Structure) { }

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
		}
	}
}