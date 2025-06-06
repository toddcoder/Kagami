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