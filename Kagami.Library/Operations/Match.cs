using Core.Collections;
using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class Match : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      var bindings = new Hash<string, IObject>();
      var _bindingX = Module.Global.Value.Bindings.Maybe[x.Id];
      if (_bindingX is (true, var bindingX))
      {
         bindings[bindingX] = x;
      }

      var _bindingY = Module.Global.Value.Bindings.Maybe[y.Id];
      if (_bindingY is (true, var bindingY))
      {
         bindings[bindingY] = y;
      }

      if (x.Match(y, bindings))
      {
         machine.CurrentFrame.Fields.SetBindings(bindings);
         return KBoolean.True.Just();
      }
      else
      {
         return KBoolean.False.Just();
      }
   }

   public override string ToString() => "match";
}