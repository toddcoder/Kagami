using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class Put : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      var text = stringOf(value);
      machine.Context.Put(text);

      return KString.StringObject(text).Just();
   }

   public override string ToString() => "put";
}