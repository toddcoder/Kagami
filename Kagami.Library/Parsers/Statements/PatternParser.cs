using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class PatternParser : StatementParser
	{
/*		static IMatched<Unit> getDataTypes(ParseState state)
		{

		}*/

		public override string Pattern => $"^ /'pattern' /(|s+|) /({REGEX_CLASS}) /'('";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			var name = tokens[3].Text;
			state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class, Color.OpenParenthesis);

			state.CreateReturnType();

			var result =
				from parameters in getParameters(state)
				from block in getAnyBlock(state)
				select (parameters, block);
			if (result.Out(out var anyResult, out var original))
			{
				state.RemoveReturnType();
				state.RegisterPattern(name);
				state.AddStatement(new Pattern(name, anyResult.parameters, anyResult.block));

				return Unit.Matched();
			}
			else
			{
				state.RemoveReturnType();
				return original.Unmatched<Unit>();
			}
		}
	}
}