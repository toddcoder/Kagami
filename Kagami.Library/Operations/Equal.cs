using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class Equal : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y) => Boolean.BooleanObject(x.IsEqualTo(y)).Just();

   public override string ToString() => "equal";
}