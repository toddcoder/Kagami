using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class HasAnyJunctions : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is Arguments arguments)
      {
         return KBoolean.BooleanObject(arguments.HasAnyJunctions).Just();
      }
      else
      {
         return KBoolean.False.Just();
      }
   }

   public override string ToString() => "has.any.junctions";
}