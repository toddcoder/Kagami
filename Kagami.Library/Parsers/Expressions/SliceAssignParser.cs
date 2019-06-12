using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class SliceAssignParser : SymbolParser
	{
		const string FIELD_SOURCE = "__$0";
		const string FIELD_INDEX = "__$1";

		public SliceAssignParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /'{'";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.BeginTransaction();
			state.Colorize(tokens, Color.Structure);

			var result =
				from indexes in getExpression(state, "^ /(/s*) /'}' /(/s*) /'='", builder.Flags, Color.Whitespace, Color.Structure,
					Color.Whitespace, Color.Structure)
				from values in getExpression(state, builder.Flags)
				select new SliceAssignSymbol(indexes, values);
			if (result.Out(out var symbol, out var original))
			{
				builder.Add(symbol);
				state.CommitTransaction();

				return Unit.Matched();
			}
			else
			{
				state.RollBackTransaction();
				return original.Unmatched<Unit>();
			}
		}
	}
}