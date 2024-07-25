using Kagami.Library.Nodes.Symbols;
using Core.Monads;

namespace Kagami.Library.Parsers.Expressions
{
   public class BNotParser : SymbolParser
   {
      public BNotParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'bnot' /b";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Operator);
         builder.Add(new BNotSymbol());

         return Unit.Matched();
      }
   }
}