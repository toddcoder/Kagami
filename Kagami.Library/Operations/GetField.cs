using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Nodes.NodeFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class GetField(string fieldName) : Operation
{
   protected string fieldName = fieldName;

   public override Optional<IObject> Execute(Machine machine)
   {
      var _field = machine.Find(fieldName, true);
      if (_field is (true, var field))
      {
         machine.LastField = field;
         machine.LastFieldName = fieldName;
         Module.Global.Value.RetrievedFields[field.Value.Id] = fieldName;

         var value = field.Value;

         switch (value)
         {
            case Objects.Some some:
            {
               machine.LastSome = (fieldName, some);
               break;
            }
            case Objects.Success success:
            {
               machine.LastSuccess = (fieldName, success);
               break;
            }
            case Singleton { CachedValue: (true, var cachedValue) }:
            {
               IObject newValue;
               if (cachedValue is Lambda lambda)
               {
                  newValue = lambda.Invoke();
               }
               else
               {
                  newValue = cachedValue;
               }

               var className = newValue.ClassName;
               if (Module.Global.Value.Class(className) is (true, var cls))
               {
                  var typeConstraint = new TypeConstraint([cls]);
                  field.TypeConstraint = typeConstraint;
                  field.Value = newValue;

                  return newValue.Just();
               }
               else
               {
                  return classNotFound(className);
               }
            }
            case Singleton:
            {
               var lazyFieldName = lazyName(fieldName);
               if (machine.Find(lazyFieldName, true) is (true, { Value: Lambda lambda }))
               {
                  var result = lambda.Invoke();
                  if (Module.Global.Value.Class(result.ClassName) is (true, var cls))
                  {
                     var typeConstraint = new TypeConstraint([cls]);
                     if (machine.Find(fieldName, true) is (true, var singletonField))
                     {
                        singletonField.TypeConstraint = typeConstraint;
                        singletonField.Value = result;

                        return result.Just();
                     }
                     else
                     {
                        return fieldNotFound(fieldName);
                     }
                  }
                  else
                  {
                     return classNotFound(result.ClassName);
                  }
               }
               else
               {
                  return fieldNotFound(lazyFieldName);
               }
            }
            case Definition definition:
            {
               return definition.Lambda.Invoke().Just();
            }
         }

         return value.Just();
      }
      else if (_field.Exception is (true, var exception))
      {
         machine.LastField = nil;
         machine.LastFieldName = nil;
         return exception;
      }
      else
      {
         _field = machine.Find("self", true);
         if (_field is (true, var self))
         {
            return sendMessage(self.Value, fieldName.get(), Arguments.Empty).Just();
         }

         machine.LastField = nil;
         machine.LastFieldName = nil;
         return fieldNotFound(fieldName);
      }
   }

   public override string ToString() => $"get.field({fieldName})";
}