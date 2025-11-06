using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class BackPipeline : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      switch (x)
      {
         case Lambda lambda when y is KTuple tuple:
         {
            return lambda.Invoke(tupleToArray(tuple)).Just();
         }
         case Lambda lambda:
         {
            return lambda.Invoke(y).Just();
         }
         case IMayInvoke mayInvoke when y is KTuple tuple:
         {
            return mayInvoke.Invoke(tupleToArray(tuple)).Just();
         }
         case IMayInvoke mayInvoke:
         {
            return mayInvoke.Invoke(y).Just();
         }
         case Message message:
         {
            return classOf(y).SendMessage(y, message).Just();
         }
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