using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.RegularExpressions;
using Core.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class SkipTakeOperatorParser : SymbolParser
	{
		public SkipTakeOperatorParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /'{'";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			if (state.Source.Keep(state.Index).IsMatch("/s+ $"))
				return notMatched<Unit>();
			else
			{
				state.Colorize(tokens, Color.Structure);

				if (getSkipTakeItems(state).Out(out var arguments, out var original))
				{
					builder.Add(new SkipTakeOperatorSymbol(arguments));
					return Unit.Matched();
				}
				else
					return original.UnmatchedOnly<Unit>();
			}
		}
	}
}