using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingIfNot(Lambda lambda) : StreamingAction
{
   public override StreamingCondition Execute(StreamingState state)
   {
      if (lambda.Invoke(state.Next).IsTrue)
      {
         return new StreamingCondition.Skipping();
      }
      else
      {
         return new StreamingCondition.Continuing(state.Next);
      }
   }

   public override string ToString() => $"ifNot({lambda.Image})";
}