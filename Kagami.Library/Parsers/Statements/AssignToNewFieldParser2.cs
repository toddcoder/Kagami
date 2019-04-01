using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class AssignToNewFieldParser2 : EndingInExpressionParser
	{
		Expression comparisand;

		public override string Pattern => "^ /'|' /(|s+|)";

		public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
		{
			state.Colorize(tokens, Color.Structure, Color.Whitespace);

			var result =
				from comparisand in getExpression(state, ExpressionFlags.Comparisand)
				from scanned in state.Scan("^ /(|s|) /'=' -(> '=')", Color.Whitespace, Color.Structure)
				select comparisand;
			if (result.Out(out comparisand, out var original))
				return Unit.Matched();
			else
				return original.Unmatched<Unit>();
		}

		public override IMatched<Unit> Suffix(ParseState state, Expression expression)
		{
			state.AddStatement(new AssignToNewField2(comparisand, expression));
			return Unit.Matched();
		}
	}
}