using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class MessageParser : SymbolParser
	{
		public MessageParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => $"^ /(|s|) /'?' /({REGEX_FUNCTION_NAME}) /'('?";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			var selector = tokens[3].Text;
			var parameterDelimiter = tokens[4].Text;
			var parseArguments = true;
			if (parameterDelimiter.IsEmpty())
			{
				selector = selector.get();
				parseArguments = false;
			}
			else if (selector.EndsWith("="))
			{
				selector = selector.Skip(-1).set();
				parseArguments = true;
			}

			state.Colorize(tokens, Color.Whitespace, Color.Message, Color.Message, Color.Structure);

			if (!parseArguments)
			{
				builder.Add(new MessageSymbol(selector, new Expression[0], none<LambdaSymbol>()));
				return Unit.Matched();
			}
			else if (getArgumentsPlusLambda(state, builder.Flags).If(out var tuple, out var original))
			{
				var (arguments, lambda) = tuple;
				builder.Add(new MessageSymbol(selector, arguments, lambda));

				return Unit.Matched();
			}
			else
				return original.Unmatched<Unit>();
      }
	}
}