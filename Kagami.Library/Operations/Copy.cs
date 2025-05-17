using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class Copy : Operation
{
   protected int index;

   public Copy(int index) => this.index = index;

   public override Optional<IObject> Execute(Machine machine)
   {
      return machine.CurrentFrame.Copy(index).Optional();
   }

   public override string ToString() => $"copy({index})";
}