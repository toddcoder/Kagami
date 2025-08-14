using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingEach(Lambda lambda) : StreamingAction
{
   public override StreamingCondition Execute(StreamingState state)
   {
      var result = lambda.Invoke(state.Next);
      return new StreamingCondition.Continuing(KUnit.Value);
   }
}