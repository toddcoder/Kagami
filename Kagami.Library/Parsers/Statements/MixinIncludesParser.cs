using System.Collections.Generic;
using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class MixinIncludesParser : StatementParser
	{
		List<Mixin> mixins;

		public MixinIncludesParser(List<Mixin> mixins)
		{
			this.mixins = mixins;
		}

		public override string Pattern => "^ /'includes' /b";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			state.Colorize(tokens, Color.Keyword);

			while (state.More)
			{
				var parser = new MixinNameParser(mixins);
				if (parser.Scan(state).If(out _, out var anyException)) { }
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
	}
}