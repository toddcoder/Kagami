using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingFirstPredicate(Lambda predicate) : StreamingAction
{
   public override StreamingCondition Execute(StreamingState state)
   {
      if (predicate.Invoke(state.Next).IsTrue)
      {
         return new StreamingCondition.Terminated(state.Next);
      }
      else
      {
         return new StreamingCondition.Skipping();
      }
   }

   public override string ToString() => $"first({predicate})";
}