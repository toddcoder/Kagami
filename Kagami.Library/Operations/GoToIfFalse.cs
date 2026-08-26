using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class GoToIfFalse(bool unwrap = true) : AddressedOperation
{
   protected Predicate<IBoolean> predicate = b => !b.IsTrue;

   public override string ToString() => $"goto.if.false({address})";

   public override Optional<IObject> Execute(Machine machine)
   {
      increment = false;

      var _x = machine.Pop();
      if (_x is (true, var x))
      {
         switch (x)
         {
            case Objects.Some some:
            {
               if (some.IsTrue)
               {
                  if (unwrap)
                  {
                     var _result =
                        from fieldName in Module.Global.Value.RetrievedFields.Maybe[some.Id]
                        from fieldValue in machine.Find(fieldName, true)
                        from classValue in Module.Global.Value.Class(some.Value.ClassName)
                        select (classValue, fieldValue, fieldName);
                     if (_result is (true, var (baseClass, field, name)))
                     {
                        if (machine.CurrentFrame.Fields.ContainsKey(name))
                        {
                           field.TypeConstraint = new TypeConstraint([baseClass]);
                           field.Value = some.Value;
                        }
                        else
                        {
                           machine.CurrentFrame.Fields.New(name, FieldType.Assignment, some.Value, mutable: true);
                        }
                     }
                  }

                  increment = true;
                  return nil;
               }
               else
               {
                  return machine.GoTo(address) ? nil : badAddress(address);
               }
            }
            case Objects.Success success:
            {
               if (success.IsTrue)
               {
                  if (unwrap)
                  {
                     var _result =
                        from fieldName in Module.Global.Value.RetrievedFields.Maybe[success.Id]
                        from fieldValue in machine.Find(fieldName, true)
                        from classValue in Module.Global.Value.Class(success.Value.ClassName)
                        select (classValue, fieldValue);
                     if (_result is (true, var (baseClass, field)))
                     {
                        field.TypeConstraint = new TypeConstraint([baseClass]);
                        field.Value = success.Value;
                     }
                  }

                  increment = true;
                  return nil;
               }
               else
               {
                  return machine.GoTo(address) ? nil : badAddress(address);
               }
            }
            case IBoolean bx when predicate(bx):
               return machine.GoTo(address) ? nil : badAddress(address);
            case IBoolean or Before:
               increment = true;
               return nil;
            case Junction junction:
            {
               return KBoolean.BooleanObject(!junction.IsTrue).Just();
            }
            default:
               return incompatibleClasses(x, "Boolean");
         }
      }
      else
      {
         return _x.Exception;
      }
   }
}