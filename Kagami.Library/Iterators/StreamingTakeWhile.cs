using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingTakeWhile(Lambda lambda) : StreamingAction
{
   public override StreamingCondition Execute(StreamingState state)
   {
      return lambda.Invoke(state.Next).IsTrue ? new StreamingCondition.Continuing(state.Next) : new StreamingCondition.Finished();
   }

   public override string ToString() => $"takeWhile({lambda.Image})";
}