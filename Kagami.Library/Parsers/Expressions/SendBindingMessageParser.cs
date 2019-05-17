using Core.Monads;
using Core.Strings;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class SendBindingMessageParser : SymbolParser
	{
		public SendBindingMessageParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => $"^ /(/s*) /('$') /({REGEX_FUNCTION_NAME}) /'('?";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			var name = tokens[3].Text;
			var parameterDelimiter = tokens[4].Text;
			var parseArguments = true;
			if (parameterDelimiter.IsEmpty())
			{
				name = name.get();
				parseArguments = false;
			}
			else if (name.EndsWith("="))
			{
				name = name.Drop(-1).set();
				parseArguments = true;
			}

			state.Colorize(tokens, Color.Whitespace, Color.Message, Color.Message, Color.Structure);

			if (!parseArguments)
			{
				Selector selector = name;
				builder.Add(new SendBindingMessageSymbol(selector));
				return Unit.Matched();
			}
			else if (getArgumentsPlusLambda(state, builder.Flags).Out(out var tuple, out var original))

			{
				var (arguments, lambda) = tuple;
				var selector = name.Selector(arguments.Length);
				builder.Add(new SendBindingMessageSymbol(selector, lambda, arguments));

				return Unit.Matched();
			}
			else
				return original.Unmatched<Unit>();
		}
	}
}