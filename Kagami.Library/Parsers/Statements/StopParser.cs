using Core.Monads;
using Kagami.Library.Nodes.Statements;

namespace Kagami.Library.Parsers.Statements
{
	public class StopParser : StatementParser
	{
		public override string Pattern => "^ /'stop' /b";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			state.Colorize(tokens, Color.Keyword);
			state.AddStatement(new Stop());

			return Unit.Matched();
		}
	}
}