using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class IfParser : ExpressionBlockParser
	{
		bool mutable;
		string fieldName;
		bool assignment;

		public override string Pattern => $"^ (/('var' | 'let') /(|s|) /({REGEX_FIELD}) /(|s|) /'=' /(|s|))? /'if' -(> '.') /b";

		public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
		{
			mutable = tokens[1].Text == "var";
			fieldName = tokens[3].Text;
			assignment = fieldName.IsNotEmpty();
			state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure,
				Color.Whitespace, Color.Keyword);

			return Unit.Matched();
		}

		public override IMatched<Unit> Suffix(ParseState state, Expression expression, Block block)
		{
			var elseIf = none<If>();
			var elseIfParser = new ElseIfParser(fieldName, mutable, assignment);
			if (elseIfParser.Scan(state).If(out _, out var mbElseIfException))
				elseIf = elseIfParser.If;
			else if (mbElseIfException.If(out var elseIfException))
				return failedMatch<Unit>(elseIfException);

			var elseBlock = none<Block>();
			var elseParser = new ElseParser();
			if (elseParser.Scan(state).If(out _, out var mbElseException))
				elseBlock = elseParser.Block;
			else if (mbElseException.If(out var elseException))
				return failedMatch<Unit>(elseException);

			state.AddStatement(new If(expression, block, elseIf, elseBlock, fieldName, mutable, assignment, true));
			return Unit.Matched();
		}
	}
}