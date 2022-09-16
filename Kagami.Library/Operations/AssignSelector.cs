using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class AssignSelector : OneOperandOperation
   {
      protected Selector selector;
      protected bool overriding;

      public AssignSelector(Selector selector, bool overriding)
      {
         this.selector = selector;
         this.overriding = overriding;
      }

      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         var createNewField = false;
         foreach (var subSelector in selector.AllSelectors())
         {
            if (createNewField)
            {
               if (machine.CurrentFrame.Fields.New(subSelector, overriding).If(out _, out var exception))
               {
               }
               else
               {
                  return failedMatch<IObject>(exception);
               }
            }

            if (machine.Assign(subSelector, value, overriding).If(out _, out var exception1))
            {
               createNewField = true;
            }
            else
            {
               return failedMatch<IObject>(exception1);
            }
         }

         return notMatched<IObject>();
      }

      public override string ToString() => $"assign.selector({selector.Image}, {overriding})";
   }
}