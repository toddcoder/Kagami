using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class Cons : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      if (y is KArray yArray)
      {
         return x switch
         {
            Objects.Some some => new KArray(some.Value).Concatenate(yArray).Just(),
            Objects.Success success => new KArray(success.Value).Concatenate(yArray).Just(),
            KNil => fail("Can't cons nil"),
            Objects.Failure => x.Just(),
            _ => new KArray(x).Concatenate(yArray).Just()
         };
      }
      else
      {
         return new KArray([x, y]);
      }
   }

   public override string ToString() => "cons";
}