using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class ImplicitMessageParser : SymbolParser
	{
		public override string Pattern => "^ /(|s|) /['!&*<>:'] -(> [/s')='])";

		public ImplicitMessageParser(ExpressionBuilder builder) : base(builder) { }

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			var source = tokens[2].Text;
			state.Colorize(tokens, Color.Whitespace, Color.Operator);

			if (getValue(state, builder.Flags).Out(out var symbol, out var original))
			{
				var fieldName = "__$0";
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
						{
							fieldName = "__$1";
							tuple = (fieldName, symbol).Some();
							state.RightZipExpression = tuple;
						}

						break;
					case "<":
						state.LeftFoldExpression = symbol.Some();
						break;
					case ">":
						state.RightFoldExpression = symbol.Some();
						break;
					case ":":
						state.BindExpression = tuple;
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