using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class NewInternalList : Operation
   {
      public override IMatched<IObject> Execute(Machine machine)
      {
         if (machine.Pop().If(out var y, out var exception))
            if (machine.IsEmpty)
               return new Tuple(y).Matched<IObject>();
            else if (machine.Pop().If(out var x, out exception))
               if (x is InternalList list)
               {
                  list.Add(y);
                  return list.Matched<IObject>();
               }
               else
                  return new InternalList(x, y).Matched<IObject>();
            else
               return failedMatch<IObject>(exception);
         else
            return failedMatch<IObject>(exception);
      }

      public override string ToString() => "new.internal.list";
   }
}