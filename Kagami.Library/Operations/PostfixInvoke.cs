using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class PostfixInvoke : TwoOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         if (y is Arguments arguments)
         {
            if (x is IInvokableObject io)
            {
               Invoke.InvokeInvokableObject(machine, io, arguments);
               return notMatched<IObject>();
            }

            if (Module.Global.Class(x.ClassName).If(out var cls))
               if (cls.RespondsTo("invoke"))
                  return cls.SendMessage(x, "invoke", arguments).Matched();
               else
                  return failedMatch<IObject>(messageNotFound(cls, "invoke"));
            else
               return failedMatch<IObject>(classNotFound(x.ClassName));
         }
         else
            return failedMatch<IObject>(incompatibleClasses(y, "Tuple"));
      }

      public override bool Increment => false;

      public override string ToString() => "postfix.invoke";
   }
}