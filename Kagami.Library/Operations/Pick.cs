using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class Pick : Operation
   {
      protected int index;

      public Pick(int index) => this.index = index;

      public override IMatched<IObject> Execute(Machine machine)
      {
         return machine.CurrentFrame.Pick(index).Map(i => i.Matched()).DefaultTo(notMatched<IObject>);
      }

      public override string ToString() => $"pick({index})";
   }
}