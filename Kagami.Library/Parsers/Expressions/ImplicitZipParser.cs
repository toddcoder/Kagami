using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class ImplicitZipParser : SymbolParser
	{
		public override string Pattern => "^ /(|s|) /'*' -(> [/s')='])";

		public ImplicitZipParser(ExpressionBuilder builder) : base(builder) { }

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Operator);

			if (getValue(state, builder.Flags).Out(out var symbol, out var original))
			{
				var fieldName = "__$0";
				var tuple = (fieldName, symbol).Some();
				if (state.LeftZipExpression.IsNone)
					state.LeftZipExpression = tuple;
				else
				{
					fieldName = "__$1";
					tuple = (fieldName, symbol).Some();
					state.RightZipExpression = tuple;
				}

				builder.Add(new FieldSymbol(fieldName));

				return Unit.Matched();
			}

			else
				return original.Unmatched<Unit>();
		}
	}
}