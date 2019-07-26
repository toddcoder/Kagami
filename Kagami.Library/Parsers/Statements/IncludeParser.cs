using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class IncludeParser : StatementParser
	{
		public override string Pattern => $"^ /'include' /(|s+) /({REGEX_CLASS})";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			return Unit.Matched();
		}
	}
}