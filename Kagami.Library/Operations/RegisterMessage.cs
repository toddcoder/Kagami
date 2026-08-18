using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class RegisterMessage(Selector selector, Func<IObject, Message, IObject> func) : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is Class @class)
      {
         try
         {
            var baseClass = classOf(@class.ClassName);
            baseClass.RegisterMessage(selector, func);

            return @class;
         }
         catch (Exception exception)
         {
            return exception;
         }
      }
      else
      {
         return expectedType("Class");
      }
   }

   public override string ToString() => $"register.message({selector})";
}