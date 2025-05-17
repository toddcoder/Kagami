using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class Image : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => String.StringObject(value.Image).Just();

   public override string ToString() => "image";
}