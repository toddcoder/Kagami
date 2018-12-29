using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class MapExpressionParser : SymbolParser
	{
		public override string Pattern => "^ /(|s|) /['!&*;'] -(> /s+)";

		public MapExpressionParser(ExpressionBuilder builder) : base(builder) { }

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			var source = tokens[2].Text;
			state.Colorize(tokens, Color.Whitespace, Color.Operator);

			if (getValue(state, builder.Flags).Out(out var symbol, out var original))
			{
				var fieldName = newLabel("item");
				var tuple = (fieldName, symbol).Some();
				switch (source)
				{
					case "!":
						state.MapExpression = tuple;
						break;
					case "&":
						state.IfExpression = tuple;
						break;
					case "*":
						if (state.LeftZipExpression.IsNone)
							state.LeftZipExpression = tuple;
						else
							state.RightZipExpression = tuple;
						break;
					case ";":
						state.FoldExpression = tuple;
						break;
				}

				builder.Add(new FieldSymbol(fieldName));
				return Unit.Matched();
			}
			else
				return original.Unmatched<Unit>();
		}
	}
}