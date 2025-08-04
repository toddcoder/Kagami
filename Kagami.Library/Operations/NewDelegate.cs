using Core.Monads;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class NewDelegate(string className, string delegateClassName) : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      var _class = Module.Global.Value.Class(className);
      if (_class is (true, UserClass userClass))
      {
         var _delegateClass = Module.Global.Value.Class(delegateClassName);
         if (_delegateClass)
         {
            userClass.RegisterDelegate(delegateClassName, value);
            return nil;
         }
         else
         {
            return classNotFound(delegateClassName);
         }
      }
      else
      {
         return classNotFound(className);
      }
   }
}