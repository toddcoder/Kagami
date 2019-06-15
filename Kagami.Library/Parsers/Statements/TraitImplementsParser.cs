using Kagami.Library.Classes;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class TraitImplementsParser : StatementParser
	{
		Hash<string, TraitClass> traits;

		public TraitImplementsParser(Hash<string, TraitClass> traits) => this.traits = traits;

		public override string Pattern => "^ /'implements' /b";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			state.Colorize(tokens, Color.Keyword);

			while (state.More)
			{
				var parser = new TraitNameParser(traits);
				if (parser.Scan(state).If(out _, out var anyException))
				{
					if (!parser.More)
						break;
				}
				else if (anyException.If(out var exception))
					return failedMatch<Unit>(exception);
				else
					break;
			}

			return Unit.Matched();
		}
	}
}