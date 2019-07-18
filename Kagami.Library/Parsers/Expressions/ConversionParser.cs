using Core.Monads;
using Kagami.Library.Nodes.Symbols;

namespace Kagami.Library.Parsers.Expressions
{
	public class ConversionParser : EndingInValueParser
	{
		string message;

		public ConversionParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /(|s|) /('int' | 'float' | 'byte' | 'long' | 'complex' | 'rational') /(|s+|)";

		public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
		{
			message = tokens[2].Text;
			state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace);

			return Unit.Matched();
		}

		public override IMatched<Unit> Suffix(ParseState state, Symbol value)
		{
			builder.Add(new ConversionSymbol(message, value));
			return Unit.Matched();
		}
	}
}