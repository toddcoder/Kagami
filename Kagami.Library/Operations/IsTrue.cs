using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class IsTrue : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is UserObject userObject)
      {
         return KBoolean.BooleanObject(sendMessage(userObject, "isTrue".get()).IsTrue).Just();
      }
      else
      {
         return KBoolean.BooleanObject(value.IsTrue).Just();
      }
   }

   public override string ToString() => "is.true";
}