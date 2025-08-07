using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingIf(Lambda lambda) : StreamingAction
{
   public override StreamingCondition Execute(StreamingState state)
   {
      if (lambda.Invoke(state.Next).IsTrue)
      {
         return new StreamingCondition.Continuing(state.Next);
      }
      else
      {
         return new StreamingCondition.Skipping();
      }
   }

   public override string ToString() => $"if({lambda.Image})";
}