using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class PostfixInvoke : TwoOperandOperation
{
   protected bool increment;

   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      switch (y)
      {
         case Arguments arguments when x is CompositeLambda compositeLambda:
         {
            var result = compositeLambda.Invoke(arguments.Value);
            increment = true;

            return result.Just();
         }
         case Arguments arguments when x is IInvokableObject io:
            Invoke.InvokeInvokableObject(machine, io, arguments);
            return nil;
         case Arguments arguments when Module.Global.Value.Class(x.ClassName) is (true, var cls):
         {
            if (cls.RespondsTo("invoke"))
            {
               return cls.SendMessage(x, "invoke", arguments).Just();
            }
            else
            {
               return messageNotFound(cls, "invoke");
            }
         }

         case Arguments:
            return classNotFound(x.ClassName);
         default:
            return incompatibleClasses(y, "Tuple");
      }
   }

   public override bool Increment => increment;

   public override string ToString() => "postfix.invoke";
}