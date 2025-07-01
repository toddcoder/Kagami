using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class NewBinding(string name) : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      Module.Global.Value.Bindings[value.Id] = name;
      return value.Just();
   }

   public override string ToString() => "new.binding";
}