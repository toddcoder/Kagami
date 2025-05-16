using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class Pick : Operation
{
   protected int index;

   public Pick(int index) => this.index = index;

   public override Optional<IObject> Execute(Machine machine) => machine.CurrentFrame.Pick(index).Optional();

   public override string ToString() => $"pick({index})";
}