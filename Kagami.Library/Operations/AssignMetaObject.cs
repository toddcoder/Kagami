using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

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
	         var selector = metaClassName.Selector(0);
	         if (Machine.Current.Find(selector).If(out var field, out var mbException))
	         {
		         if (field.Value is IInvokableObject io)
		         {
			         var invokable = io.Invokable;
			         if (machine.Invoke(invokable, Arguments.Empty, 0).If(out var obj, out mbException))
			         {
				         ((UserClass)targetClass).MetaObject = ((UserObject)obj).Some();
				         return notMatched<IObject>();
                  }
						else if (mbException.If(out var exception))
				         return failedMatch<IObject>(exception);
			         else
				         return $"Couldn't construct metaobject {metaClassName}".FailedMatch<IObject>();
		         }

            }
            else if (mbException.If(out var exception))
		         return failedMatch<IObject>(exception);
	         else
		         return $"Couldn't find metaclass {metaClassName}".FailedMatch<IObject>();
            return notMatched<IObject>();
         }
         else
            return failedMatch<IObject>(classNotFound(className));
      }

      public override string ToString() => "assign.meta.object";
   }
}