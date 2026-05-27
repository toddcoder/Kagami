using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class AssignSelector : OneOperandOperation
{
   protected Selector selector;
   protected bool overriding;

   public AssignSelector(Selector selector, bool overriding)
   {
      this.selector = selector;
      this.overriding = overriding;
   }

   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      foreach (var subSelector in selector.AllSelectors().Distinct())
      {
         if (!machine.CurrentFrame.Fields.SelectorExists(subSelector))
         {
            var _fields = machine.CurrentFrame.Fields.NewSelector(subSelector, FieldType.Function, overriding: overriding);
            if (!_fields)
            {
               return _fields.Exception;
            }
         }

         var _field = machine.Assign(subSelector, value, overriding);
         if (!_field)
         {
            return _field.Exception;
         }
      }

      return nil;
   }

   public override string ToString() => $"assign.selector({selector.Image}, {overriding})";
}