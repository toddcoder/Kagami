using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class NewTag(string tag) : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      Module.RegisterTag(value, tag);
      return value.Just();
   }

   public override string ToString() => $"new.tag({tag})";
}