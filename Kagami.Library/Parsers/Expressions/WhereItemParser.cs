using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class WhereItemParser : EndingInExpressionParser
	{
		public WhereItemParser(ExpressionBuilder builder) : base(builder,
			ExpressionFlags.OmitColon | ExpressionFlags.OmitComma | ExpressionFlags.Comparisand) { }

		string propertyName;

		public override string Pattern => $"^ /(|s|) /({REGEX_FIELD}) /(|s|) /':'";

		public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
		{
			propertyName = tokens[2].Text;
			state.Colorize(tokens, Color.Whitespace, Color.Message, Color.Whitespace, Color.Structure);

			return Unit.Matched();
		}

		public override IMatched<Unit> Suffix(ParseState state, Expression expression)
		{
			Expression = expression;
			return Unit.Matched();
		}

		public string PropertyName => propertyName;

		public Expression Expression { get; set; }
	}
}