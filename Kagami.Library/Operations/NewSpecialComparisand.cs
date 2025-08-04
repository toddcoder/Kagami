using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class NewSpecialComparisand(SpecialComparisandDirection direction) : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => new SpecialComparisand(direction, value);

   public override string ToString() => $"new.special.comparisand({direction})";
}