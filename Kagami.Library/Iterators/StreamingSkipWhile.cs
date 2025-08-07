using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingSkipWhile(Lambda lambda) : StreamingAction
{
   protected bool skippingStopped;

   public override StreamingCondition Execute(StreamingState state)
   {
      if (skippingStopped || !lambda.Invoke(state.Next).IsTrue)
      {
         skippingStopped = true;
         return new StreamingCondition.Continuing(state.Next);
      }
      else
      {
         return new StreamingCondition.Skipping();
      }
   }

   public override string ToString() => $"skipWhile({lambda.Image})";
}