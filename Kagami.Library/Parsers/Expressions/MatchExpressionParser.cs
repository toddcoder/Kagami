using System.Collections.Generic;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class MatchExpressionParser : SymbolParser
	{
		static IMatched<(Expression, Expression)> getMatchItem(ParseState state)
		{
			if (state.Scan("^ /(/s*) /'}'", Color.Whitespace, Color.Structure).IsMatched)
			{
				return notMatched<(Expression, Expression)>();
			}
			else
			{
				var matchItem =
					from key in getExpression(state, ExpressionFlags.Comparisand | ExpressionFlags.OmitNameValue)
					from _ in state.Scan("^ /(/s*) /'=>' /(/s*)", Color.Whitespace, Color.Operator, Color.Whitespace)
					from expression in getExpression(state, ExpressionFlags.OmitComma)
					select (key, expression);
				if (matchItem.IsMatched)
				{
					state.Scan("^ /(/s*) /',' /(/s*)", Color.Whitespace, Color.Structure, Color.Whitespace);
				}

				return matchItem;
			}
		}

		public MatchExpressionParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /(|s|) /'match' /(/s*) /'{' /(/s*)";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Structure, Color.Whitespace);

			var matchItems = new List<(Expression, Expression)>();

			while (state.More)
			{
				if (getMatchItem(state).If(out var matchItem, out var anyException))
				{
					matchItems.Add(matchItem);
				}
				else if (anyException.If(out var exception))
				{
					return failedMatch<Unit>(exception);
				}
				else
				{
					break;
				}
			}

			builder.Add(new MatchExpressionSymbol(matchItems.ToArray()));

			return Unit.Matched();
		}
	}
}