using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class AssignFunction(Selector selector, Lambda lambda, bool isOverriding) : Operation
{
   protected Selector selector = selector;

   public override Optional<IObject> Execute(Machine machine)
   {
      lambda.Capture(machine);
      foreach (var subSelector in Selector.GetAllSelectors(selector).Distinct())
      {
         if (!machine.CurrentFrame.Fields.SelectorExists(subSelector))
         {
            var _fields = machine.CurrentFrame.Fields.NewSelector(subSelector, FieldType.Function, overriding: isOverriding);
            if (!_fields)
            {
               return _fields.Exception;
            }

            var _field = machine.Assign(subSelector, lambda, isOverriding);
            if (!_field)
            {
               return _field.Exception;
            }
         }
      }

      return nil;
   }

   public override string ToString() => $"assign.function({selector}, {lambda.Image}, {isOverriding})";
}