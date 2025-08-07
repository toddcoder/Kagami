using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingEach(Lambda lambda) : StreamingAction
{
   public override StreamingCondition Execute(StreamingState state)
   {
      lambda.Invoke(state.Next);
      return new StreamingCondition.Continuing(state.Next);
   }

   public override string ToString() => $"each({lambda.Image})";
}