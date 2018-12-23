using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;
using Standard.Types.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class CharParser : SymbolParser
	{
		public CharParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /(|s|) /(\"'\" ('\\' ['xu'] ['a-f0-9']1%6 | '\\'? .) \"'\")";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Char);
			var source = tokens[2].Text.Skip(1).Skip(-1);

			switch (source.Length)
			{
				case 1:
					builder.Add(new CharSymbol(source[0]));
					break;
				case 2 when source.StartsWith("\\"):
					if (fromBackslash(source[1]).If(out var ch, out var exception))
						builder.Add(new CharSymbol(ch));
					else
						return failedMatch<Unit>(exception);

					break;
				default:
					if (fromHex(source.Skip(2)).Out(out ch, out var original))
						builder.Add(new CharSymbol(ch));
					else
						return original.Map(c => Unit.Value);

					break;
			}

			return Unit.Matched();
		}
	}
}