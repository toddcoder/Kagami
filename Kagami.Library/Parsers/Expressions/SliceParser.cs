using System.Collections.Generic;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class SliceParser : SymbolParser
	{
		public class SkipTake
		{
			public IMaybe<Expression> Skip { get; set; } = none<Expression>();

			public IMaybe<Expression> Take { get; set; } = none<Expression>();

			public bool Terminal { get; set; }

			public override string ToString()
			{
				return $"{Skip.FlatMap(e => e.ToString(), () => "")};{Take.FlatMap(e => e.ToString(), () => "")}";
			}
		}

		public SliceParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /'{'";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.Colorize(tokens, Color.OpenParenthesis);

			var skipTakes = new List<SkipTake>();

			while (state.More)
			{
				var skipTakeMatch = getSkipTake(state, builder.Flags | ExpressionFlags.OmitComma);
				if (skipTakeMatch.If(out var skipTake, out var anyException))
				{
					skipTakes.Add(skipTake);
					if (skipTake.Terminal)
						break;
				}
				else if (anyException.If(out var exception))
					return failedMatch<Unit>(exception);
			}

			builder.Add(new SliceSymbol(skipTakes.ToArray()));

			return Unit.Matched();
		}

		IMatched<SkipTake> getSkipTake(ParseState state, ExpressionFlags flags)
		{
			var skipTake = new SkipTake();

			var noSkipMatch = state.Scan("^ /(|s|) /','", Color.Whitespace, Color.Structure);
			if (noSkipMatch.If(out _, out var anyException)) { }
			else if (anyException.If(out var exception))
				return failedMatch<SkipTake>(exception);
			else
			{
				var skipMatch = getExpression(state, flags);
				if (skipMatch.If(out var skipExpression, out anyException))
					skipTake.Skip = skipExpression.Some();
				else if (anyException.If(out exception))
					return failedMatch<SkipTake>(exception);

				var semiOrEndMatch = state.Scan("^ /(|s|) /[';,}']", Color.Whitespace, Color.CloseParenthesis);
				if (semiOrEndMatch.If(out var semiOrEnd, out anyException))
					switch (semiOrEnd)
					{
						case "}":
							skipTake.Terminal = true;
							return skipTake.Matched();
						case ";":
							return skipTake.Matched();
					}
				else if (anyException.If(out exception))
					return failedMatch<SkipTake>(exception);
			}

			var takeMatch = getExpression(state, flags);
			if (takeMatch.If(out var takeExpression, out anyException))
				skipTake.Take = takeExpression.Some();
			else if (anyException.If(out var exception))
				return failedMatch<SkipTake>(exception);

			var endMatch = state.Scan("^ /(|s|) /['};']", Color.Whitespace, Color.CloseParenthesis);
			if (endMatch.If(out var end, out anyException))
				switch (end)
				{
					case "}":
						skipTake.Terminal = true;
						return skipTake.Matched();
				}
			else if (anyException.If(out var exception))
				return failedMatch<SkipTake>(exception);

			return skipTake.Matched();
		}
	}
}