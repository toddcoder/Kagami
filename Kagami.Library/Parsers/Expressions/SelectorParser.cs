using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;

namespace Kagami.Library.Parsers.Expressions
{
   public class SelectorParser : SymbolParser
   {
      public override string Pattern => "^ /(|s|) /'@' /(-/s+)";

      public SelectorParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
	      Selector selector = tokens[3].Text;
			state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Identifier);
			builder.Add(new PushObjectSymbol(selector));

         return Unit.Matched();
      }
   }
}