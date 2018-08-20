using Standard.Types.Maybe;

namespace Kagami.Library.Parsers.Expressions
{
   public class SelectorParser : SymbolParser
   {
      public override string Pattern => "^ /'#'";

      public SelectorParser(ExpressionBuilder builder) : base(builder) { }

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         return Unit.Matched();
      }
   }
}