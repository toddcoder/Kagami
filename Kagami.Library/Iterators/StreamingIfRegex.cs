using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingIfRegex(Regex regex) : StreamingAction
{
   public override StreamingCondition Execute(StreamingState state)
   {
      if (regex.Matches(state.Next.AsString).IsTrue)
      {
         return new StreamingCondition.Continuing(state.Next);
      }
      else
      {
         return new StreamingCondition.Skipping();
      }
   }

   public override string ToString() => $"if({regex.Image})";
}