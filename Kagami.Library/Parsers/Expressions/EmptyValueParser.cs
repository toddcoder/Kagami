using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class EmptyValueParser : SymbolParser
	{
		public EmptyValueParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /(|s|) /('{}' | '{:}' | '()' | '[]' | '⌈⌉' | '⎩⎭')";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			var source = tokens[2].Text;
			state.Colorize(tokens, Color.Whitespace, Color.Collection);

			switch (source)
			{
				case "[]":
					builder.Add(new EmptyArraySymbol());
					break;
				case "{}":
					builder.Add(new EmptyDictionarySymbol());
					break;
				case "()":
					builder.Add(new EmptyTupleSymbol());
					break;
				case "⌈⌉":
					builder.Add(new EmptyListSymbol());
					break;
				case "⎩⎭":
					builder.Add(new EmptySetSymbol());
					break;
				default:
					return notMatched<Unit>();
			}

			return Unit.Matched();
		}
	}
}