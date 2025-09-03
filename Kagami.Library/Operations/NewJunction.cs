using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class NewJunction(string junctionType) : OneOperandOperation
{
   public override string ToString() => $"new.junction({junctionType})";

   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is Sequence sequence)
      {
         return new Junction(junctionType, sequence);
      }
      else
      {
         return incompatibleClasses(value, "String");
      }
   }
}