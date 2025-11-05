using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class Put : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      machine.Context.Put(stringOf(value));
      var image = (KString)value.Image;

      return image;
   }

   public override string ToString() => "put";
}