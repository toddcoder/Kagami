using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class Iterator : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is IIterator iterator)
      {
         while (iterator.Next() && !machine.Context.Cancelled())
         {
         }

         return KUnit.Value.Just();
      }
      else
      {
         return fail("Value is not an iterator");
      }
   }

   public override string ToString() => "iterate";
}