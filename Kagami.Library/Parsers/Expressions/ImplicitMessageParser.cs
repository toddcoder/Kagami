using System.Linq;
using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class ImplicitMessageParser : SymbolParser
	{
		static string parameters(int count)
		{
			return $"({Enumerable.Range(0, count).Select(i => "_").Stringify(",")})";
		}

		public ImplicitMessageParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => $"^ /(|s|) /({REGEX_ITERATOR_FUNCTIONS}) /'^'";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			var message = tokens[2].Text;
			state.Colorize(tokens, Color.Whitespace, Color.Message, Color.Structure);

			if (getValue(state, builder.Flags).Out(out var symbol, out var original))
			{
				var parameterCount = 1;
				var fieldName = "__$0";
				switch (message)
				{
					case "for":
						message = "each";
						break;
					case "fold":
						message = "foldl";
						goto case "foldl";
					case "foldl":
						fieldName = "__$1";
						parameterCount = 2;
						break;
					case "reducel":
						fieldName = "__$1";
						parameterCount = 2;
						break;
					case "accr":
						message = "foldr";
						goto case "foldr";
					case "foldr":
					case "reducer":
						parameterCount = 2;
						break;
					case "while":
						message = "takeWhile";
						break;
					case "until":
						message = "takeUntil";
						break;
					case "z":
					case "zip":
					case "x":
					case "cross":
						if (state.ImplicitState.IsNone)
						{
							var newMessage = message == "z" || message == "zip" ? "zip(_,_)" : "cross(_,_)";
							state.ImplicitState = new ImplicitState(symbol, newMessage, 2, "__$0").Some();
							builder.Add(new FieldSymbol("__$0"));
							return Unit.Matched();
						}
						else if (state.ImplicitState.If(out var implicitState))
						{
							implicitState.Two = symbol.Some();
							builder.Add(new FieldSymbol("__$1"));
							return Unit.Matched();
						}

						break;
/*					case "seq":
						message = "seq(_)";
						break;*/
				}

				state.ImplicitState = new ImplicitState(symbol, message + parameters(parameterCount), parameterCount, fieldName).Some();
				builder.Add(new FieldSymbol(fieldName));

				return Unit.Matched();
			}
			else
			{
				return original.Unmatched<Unit>();
			}
		}
	}
}