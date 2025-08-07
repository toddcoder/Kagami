using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingTakeUntil(Lambda lambda) : StreamingAction
{
   public override StreamingCondition Execute(StreamingState state)
   {
      return lambda.Invoke(state.Next).IsTrue ? new StreamingCondition.Finished() : new StreamingCondition.Continuing(state.Next);
   }

   public override string ToString() => $"takeUntil({lambda.Image})";
}