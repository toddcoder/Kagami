using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class PrintLine : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      machine.Context.PrintLine(stringOf(value));
      var image = (KString)value.Image;

      return image;
   }

   public override string ToString() => "println";
}