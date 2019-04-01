using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class ElseIfParser : ExpressionBlockParser
	{
		string fieldName;
		bool mutable;
		bool assignment;

		public ElseIfParser(string fieldName, bool mutable, bool assignment)
		{
			this.fieldName = fieldName;
			this.mutable = mutable;
			this.assignment = assignment;
		}

		public override string Pattern => "^ /'else' /(|s+|) /'if' /b";

		public IMaybe<If> If { get; set; } = none<If>();

		public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
		{
			state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Keyword);

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

			If = new If(expression, block, elseIf, elseBlock, fieldName, mutable, assignment, false).Some();

			return Unit.Matched();
		}
	}
}