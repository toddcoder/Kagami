using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class NewJunction2(JunctionType junctionType) : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      if (x is Junction existingJunction)
      {
         return existingJunction.Append(y, junctionType);
      }
      else
      {
         return new Junction(junctionType, [x, y]);
      }
   }

   public override string ToString() => $"new.junction2({junctionType})";
}