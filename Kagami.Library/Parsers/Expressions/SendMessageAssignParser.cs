using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class SendMessageAssignParser : EndingInExpressionParser
	{
		string messageName;
		Precedence precedence;
		string operationSource;

		public SendMessageAssignParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => $"^ /(|s|) /['.@'] /({REGEX_FUNCTION_NAME}) /(|s|) /({REGEX_ASSIGN_OPS})? /'=' -(> ['=>'])";

		public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
		{
			precedence = tokens[2].Text == "." ? Precedence.SendMessage : Precedence.ChainedOperator;
			messageName = tokens[3].Text;
			operationSource = tokens[5].Text;
			state.Colorize(tokens, Color.Whitespace, Color.Structure, Color.Message, Color.Whitespace, Color.Operator,
				Color.Structure);

			return Unit.Matched();
		}

		public override IMatched<Unit> Suffix(ParseState state, Expression expression)
		{
			if (matchOperator(operationSource).Out(out var operation, out var original) || original.IsNotMatched)
			{
				var assignmentOperation = maybe(original.IsMatched, () => operation);
				builder.Add(new SendMessageSymbol(messageName.set(), precedence, assignmentOperation, expression));
				return Unit.Matched();
			}
			else
			{
				return original.Unmatched<Unit>();
			}
		}
	}
}