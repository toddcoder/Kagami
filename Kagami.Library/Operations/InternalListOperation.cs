using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public abstract class InternalListOperation : OneOperandOperation
{
   public abstract Optional<IObject> Execute(Container list);

   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is Container list)
      {
         return Execute(list);
      }
      else
      {
         return incompatibleClasses(value, "InternalList");
      }
   }
}