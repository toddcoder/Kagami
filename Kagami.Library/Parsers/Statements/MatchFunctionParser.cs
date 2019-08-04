using System.Collections.Generic;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class MatchFunctionParser : StatementParser
	{
		public override string Pattern => $"^ /('override' /s+)? /'func' /(|s+|) (/({REGEX_CLASS_GETTING}) /'.')? " +
			$"/({REGEX_FUNCTION_NAME}) /'(' /'|'";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			var className = tokens[4].Text;
			var functionName = tokens[6].Text;

			state.Colorize(tokens, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Class, Color.Structure, Color.Invokable,
				Color.OpenParenthesis, Color.Structure);

			if (getComparisandList(state).Out(out var comparisandList, out var original))
			{

			}
			else
			{
				return original.Unmatched<Unit>();
			}
		}
	}
}