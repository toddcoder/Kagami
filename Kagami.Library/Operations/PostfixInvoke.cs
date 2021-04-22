using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class PostfixInvoke : TwoOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         switch (y)
         {
            case Arguments arguments when x is IInvokableObject io:
               Invoke.InvokeInvokableObject(machine, io, arguments);
               return notMatched<IObject>();
            case Arguments arguments when Module.Global.Class(x.ClassName).If(out var cls):
               if (cls.RespondsTo("invoke"))
               {
                  return cls.SendMessage(x, "invoke", arguments).Matched();
               }
               else
               {
                  return failedMatch<IObject>(messageNotFound(cls, "invoke"));
               }

            case Arguments:
               return failedMatch<IObject>(classNotFound(x.ClassName));
            default:
               return failedMatch<IObject>(incompatibleClasses(y, "Tuple"));
         }
      }

      public override bool Increment => false;

      public override string ToString() => "postfix.invoke";
   }
}