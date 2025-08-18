using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class BackPipeline : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      switch (x)
      {
         case Lambda lambda:
            return lambda.Invoke(y).Just();
         case IMayInvoke mayInvoke:
            return mayInvoke.Invoke(y).Just();
         case Selector selector:
         {
            var _field = Machine.Current.Value.Find(selector);
            if (_field is (true, { Value: Lambda lambda }))
            {
               return lambda.Invoke(y).Just();
            }
            else if (_field.Exception is (true, var exception))
            {
               return exception;
            }
            else
            {
               return fieldNotFound(selector);
            }
         }
         default:
            return incompatibleClasses(x, "Lambda");
      }
   }

   public override string ToString() => "back.pipeline";
}