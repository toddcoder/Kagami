using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class RangeSymbol : Symbol
   {
      bool inclusive;
      bool reverse;

      public RangeSymbol(bool inclusive, bool reverse = false)
      {
         this.inclusive = inclusive;
         this.reverse = reverse;
      }

      public override void Generate(OperationsBuilder builder)
      {
         builder.NewRange(inclusive);
         if (reverse)
         {
            builder.PushInt(-1);
            builder.NewRange(true);
         }
      }

      public override Precedence Precedence => Precedence.Range;

      public override Arity Arity => Arity.Binary;

      public override string ToString()
      {
         if (reverse)
            return "--";
         else if (inclusive)
            return "++";
         else
            return "+-";
      }
   }
}