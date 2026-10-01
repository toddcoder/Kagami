using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingIfNotRegex(Regex regex) : StreamingAction
{
   public override StreamingCondition Execute(StreamingState state)
   {
      if (regex.Matches(state.Next.AsString).IsTrue)
      {
         return new StreamingCondition.Skipping();
      }
      else
      {
         return new StreamingCondition.Continuing(state.Next);
      }
   }

   public override string ToString() => $"ifNot({regex.Image})";
}