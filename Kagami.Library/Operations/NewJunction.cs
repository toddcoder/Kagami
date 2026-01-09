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
      return value switch
      {
         Sequence sequence => new Junction(junctionType, sequence),
         ICollection collection => new Junction(junctionType, new Sequence(collection.GetIterator(false).List())),
         _ => incompatibleClasses(value, "String")
      };
   }
}