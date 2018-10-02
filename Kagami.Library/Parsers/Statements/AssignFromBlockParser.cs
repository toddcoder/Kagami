using Kagami.Library.Nodes.Statements;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class AssignFromBlockParser : StatementParser
	{
		public override string Pattern => $"^ (/('var' | 'let') /(|s+|))? /({REGEX_FIELD}) /b";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			state.BeginTransaction();

			var isNew = tokens[1].Text.IsNotEmpty();
			var mutable = tokens[1].Text == "var";
			var fieldName = tokens[3].Text;
			state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Identifier);

			var result =
				from typeConstraint in parseTypeConstraint(state)
				from scanned in state.Scan($"^ /(|s|) /'=' /({REGEX_EOL})", Color.Whitespace, Color.Structure, Color.Whitespace)
				from block in getBlock(state)
				select (typeConstraint, block);

			if (result.If(out var tuple, out _))
			{
				var (typeConstraint, block) = tuple;
				state.AddStatement(new AssignToFieldWithBlock(isNew, mutable, fieldName, typeConstraint, block));
				state.CommitTransaction();

				return Unit.Matched();
			}
			else
			{
				state.RollBackTransaction();
				return notMatched<Unit>();
			}
		}
	}
}