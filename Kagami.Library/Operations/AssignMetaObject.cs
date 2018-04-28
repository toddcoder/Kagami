using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class AssignMetaObject : Operation
   {
      string className;
      string metaClassName;

      public AssignMetaObject(string className, string metaClassName)
      {
         this.className = className;
         this.metaClassName = metaClassName;
      }

      public override IMatched<IObject> Execute(Machine machine)
      {
         if (Module.Global.Class(className).If(out var targetClass))
         {
            if (Machine.Current.Find(metaClassName, true)
               .If(out var field, out var isNotMatched, out var exception))
            {
               if (field.Value is IInvokableObject io)
               {
                  var invokable = io.Invokable;
                  var result = machine.Invoke(invokable, Arguments.Empty, 0);
                  if (result.If(out var obj, out isNotMatched, out exception))
                  {
                     ((UserClass)targetClass).MetaObject = ((UserObject)obj).Some();
                     return notMatched<IObject>();
                  }
                  else if (isNotMatched)
                     return $"Couldn't construct metaobject {metaClassName}".FailedMatch<IObject>();
                  else
                     return failedMatch<IObject>(exception);
               }
            }
            else if (isNotMatched)
               return $"Couldn't find metaclass {metaClassName}".FailedMatch<IObject>();
            else
               return failedMatch<IObject>(exception);

            return notMatched<IObject>();
         }
         else
            return failedMatch<IObject>(classNotFound(className));
      }

      public override string ToString() => "assign.meta.object";
   }
}