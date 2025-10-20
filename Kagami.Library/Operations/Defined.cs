using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class Defined(string name, bool isClass) :Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      if (isClass)
      {
         return (Module.Global.Value.Class(name).Map(_ => KBoolean.True) | (() => KBoolean.False)).Just();
      }
      else
      {
         return (machine.Find(name, true).Map(_ => KBoolean.True) | (() => KBoolean.False)).Just();
      }
   }

   public override string ToString() => "defined";
}