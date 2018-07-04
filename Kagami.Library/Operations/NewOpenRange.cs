using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class NewOpenRange : TwoOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         switch (x)
         {
            case Int i:
               return new Sequence(i.Value, y).Matched<IObject>();
            case Sequence seq:
               return new Sequence(seq, y).Matched<IObject>();
            default:
               switch (y)
               {
                  case Lambda lambda:
                     return new OpenRange(x, lambda).Matched<IObject>();
                  case INumeric _:
                     var internalLambda = new InternalLambda(o => sendMessage(o[0], "+", y));
                     return new OpenRange(x, internalLambda).Matched<IObject>();
                  default:
                     return failedMatch<IObject>(incompatibleClasses(y, "Lambda"));
               }
         }
      }

      public override string ToString() => "new.open.range";
   }
}