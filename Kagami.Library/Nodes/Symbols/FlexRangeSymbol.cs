using Kagami.Library.Operations;
using Standard.Types.Booleans;
using Standard.Types.Maybe;

namespace Kagami.Library.Nodes.Symbols
{
   public class FlexRangeSymbol : Symbol
   {
      Expression from;
      LambdaSymbol by;
      IMaybe<Expression> to;
      bool inclusive;

      public FlexRangeSymbol(Expression from, LambdaSymbol by, IMaybe<Expression> to, bool inclusive)
      {
         this.from = from;
         this.by = by;
         this.to = to;
         this.inclusive = inclusive;
      }

      public override void Generate(OperationsBuilder builder)
      {
         from.Generate(builder);
         by.Generate(builder);

         if (to.If(out var t))
         {
            t.Generate(builder);
            builder.PushBoolean(inclusive);
            builder.NewFlexRange(4);
         }
         else
            builder.NewFlexRange(2);
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString()
      {
         return $"from {from} by {by}{to.FlatMap(t => $" {inclusive.Extend("to", "til")} {t}", () => "")}";
      }
   }
}