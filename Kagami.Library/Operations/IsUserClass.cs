using Core.Monads;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class IsUserClass : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => KBoolean.BooleanObject(classOf(value) is UserClass).Just();

   public override string ToString() => "is.user.class";
}