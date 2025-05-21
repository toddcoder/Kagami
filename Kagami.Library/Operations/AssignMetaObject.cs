using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class AssignMetaObject : Operation
{
   protected string className;
   protected string metaClassName;

   public AssignMetaObject(string className, string metaClassName)
   {
      this.className = className;
      this.metaClassName = metaClassName;
   }

   public override Optional<IObject> Execute(Machine machine)
   {
      if (Module.Global.Value.Class(className) is (true, var targetClass))
      {
         var selector = metaClassName.Selector(0);
         var _field = Machine.Current.Value.Find(selector);
         if (_field is (true, var field))
         {
            if (field.Value is IInvokableObject io)
            {
               var invokable = io.Invokable;
               var _obj = machine.Invoke(invokable, Arguments.Empty, 0);
               if (_obj is (true, var obj))
               {
                  ((UserClass)targetClass).MetaObject = ((UserObject)obj).Some();
                  return nil;
               }
               else if (_obj.Exception is (true, var exception))
               {
                  return exception;
               }
               else
               {
                  return fail($"Couldn't construct metaobject {metaClassName}");
               }
            }
         }
         else if (_field.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            return fail($"Couldn't find metaclass {metaClassName}");
         }

         return nil;
      }
      else
      {
         return classNotFound(className);
      }
   }

   public override string ToString() => "assign.meta.object";
}