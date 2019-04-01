using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class PopFrameWithValue : Operation
   {
      public override IMatched<IObject> Execute(Machine machine)
      {
         if (machine.PopFrame().If(out var frame, out var exception))
            if (frame.Pop().If(out var value, out exception))
               return value.Matched();
            else
               return failedMatch<IObject>(exception);
         else
            return failedMatch<IObject>(exception);
      }

      public override string ToString() => "pop.frame.with.value";
   }
}