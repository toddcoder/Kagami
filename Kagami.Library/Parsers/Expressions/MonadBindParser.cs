using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class MonadBindParser : EndingInExpressionParser
	{
		string fieldName;

		public MonadBindParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => $"^ /(|s|) /({REGEX_FIELD}) /(|s|) /':='";

		public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
		{
			fieldName = tokens[2].Text;
			state.Colorize(tokens, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure);

			return Unit.Matched();
		}

		public override IMatched<Unit> Suffix(ParseState state, Expression expression)
		{
			MonadBindSymbol = new MonadBindSymbol(fieldName, expression);
			return Unit.Matched();
		}

		public MonadBindSymbol MonadBindSymbol { get; set; }
	}
}