using Core.Monads;
using Kagami.Library.Nodes.Symbols;

namespace Kagami.Library.Parsers.Expressions
{
   public class TakeOperatorParser : SymbolParser
   {
      public TakeOperatorParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'*;'";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Operator);
         builder.Add(new TakeOperatorSymbol());

         return Unit.Matched();
      }
   }
}