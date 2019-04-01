using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class ParentClassParser : StatementParser
	{
		public override string Pattern => $"^ /'inherits' /(|s+|) /({REGEX_CLASS}) " + "/['({']?";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			var parentClassName = tokens[3].Text;
			var hasArguments = tokens[4].Length > 0;
			var initialize = tokens[4].Text == "{";
			state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class, Color.Structure);

			if (hasArguments)
			{
				if (getArguments(state, ExpressionFlags.Standard).Out(out var expressions, out var original))
				{
					Parent = (parentClassName, initialize, expressions);
					return Unit.Matched();
				}
				else if (original.IsNotMatched)
				{
					Parent = (parentClassName, initialize, new Expression[0]);
					return Unit.Matched();
				}
				else
					return original.ExceptionAs<Unit>();
			}
			else
			{
				Parent = (parentClassName, initialize, new Expression[0]);
				return Unit.Matched();
			}
		}

		public (string, bool, Expression[]) Parent { get; set; }
	}
}