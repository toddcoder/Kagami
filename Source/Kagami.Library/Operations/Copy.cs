using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class Copy : Operation
   {
      protected int index;

      public Copy(int index) => this.index = index;

      public override IMatched<IObject> Execute(Machine machine)
      {
         return machine.CurrentFrame.Copy(index).Map(i => i.Matched()).DefaultTo(notMatched<IObject>);
      }

      public override string ToString() => $"copy({index})";
   }
}