using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Booleans;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class NewRange : TwoOperandOperation
   {
      bool inclusive;

      public NewRange(bool inclusive) => this.inclusive = inclusive;

      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         switch (x)
         {
            case IRangeItem start when y is Any:
               return new Range(start, new Infinity(true), inclusive).Matched<IObject>();
            case IRangeItem start when y is IObjectCompare stop:
               return new Range(start, stop, inclusive).Matched<IObject>();
            case Range range when y is Int increment:
               return new Range(range, increment).Matched<IObject>();
            default:
               return failedMatch<IObject>(incompatibleClasses(x, "RangeItem"));
         }
      }

      public override string ToString() => (StringStream)"new.range(" / inclusive.Extend("inclusive","exclusive") / ")";
   }
}