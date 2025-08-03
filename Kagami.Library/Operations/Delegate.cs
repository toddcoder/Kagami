using Core.Monads;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class Delegate(string fieldName, string hostClassName) : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      var delegateObject = (UserObject)value;
      var _delegateClass = Module.Global.Value.Class(delegateObject.ClassName);
      if (_delegateClass is (true, UserClass delegateClass))
      {
         var _hostClass = Module.Global.Value.Class(hostClassName);
         if (_hostClass is (true, UserClass hostClass))
         {
            foreach (var selector in delegateClass.Signatures)
            {
               hostClass.RegisterMethod(selector,new InternalLambda(args=>))
            }
         }
      }
      return nil;
   }
}