using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class BindComparisand : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      Module.Global.Value.Bindings[y.Id] = x.AsString;
      return y.Just();
   }

   public override string ToString() => "bind.comparisand";
}