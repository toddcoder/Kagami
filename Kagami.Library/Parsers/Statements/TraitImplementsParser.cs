using Kagami.Library.Classes;
using Standard.Types.Collections;
using Standard.Types.Monads;
using static Standard.Types.Monads.MonadFunctions;

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
				if (parser.Scan(state).If(out _, out var mbException))
				{
					if (!parser.More)
						break;
				}
				else if (mbException.If(out var exception))
					return failedMatch<Unit>(exception);
				else
					break;
			}

			return Unit.Matched();
		}
	}
}