using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class NewFlexRange : MultipleOperandOperation
   {
      IObject from;
      Lambda by;
      IMaybe<IObject> to;
      bool inclusive;

      public NewFlexRange(int count) : base(count) => to = none<IObject>();

      public override IResult<Unit> Execute(Machine machine, int index, IObject value)
      {
         switch (index)
         {
            case 0:
               from = value;
               return Unit.Success();
            case 1:
               return match<Lambda>(value, l => by = l);
            case 2:
               return match<IObject>(value, oc => to = oc.Some());
            case 3:
               return match<Boolean>(value, b => inclusive = b.Value);
            default:
               return Unit.Success();
         }
      }

      public override IMatched<IObject> Final(Machine machine)
      {
         if (to.If(out var t))
            return new FlexRange(from, by, (IObjectCompare)t, inclusive).Matched<IObject>();
         else
            return new FlexRange(from, by).Matched<IObject>();
      }

      public override string ToString() => "new.flex.range";
   }
}