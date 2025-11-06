using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class Pipeline : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      switch (y)
      {
         case Lambda lambda when x is KTuple tuple:
         {
            return lambda.Invoke(tupleToArray(tuple)).Just();
         }
         case Lambda lambda:
         {
            return lambda.Invoke(x).Just();
         }
         case IMayInvoke mi when x is KTuple tuple:
         {
            return mi.Invoke(tupleToArray(tuple)).Just();
         }
         case IMayInvoke mi:
         {
            return mi.Invoke(x).Just();
         }
         case Message message:
         {
            return classOf(x).SendMessage(x, message).Just();
         }
         case Selector selector:
         {
            var _field = Machine.Current.Value.Find(selector);
            if (_field is (true, { Value: Lambda lambda }))
            {
               return lambda.Invoke(x).Just();
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
            return incompatibleClasses(y, "Lambda");
      }
   }

   public override string ToString() => "pipeline";
}