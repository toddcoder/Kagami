namespace Kagami.Library.Iterators;

public class StreamingSkip(int count) : StreamingAction
{
   protected int skipped;

   public override StreamingCondition Execute(StreamingState state)
   {
      if (skipped < count)
      {
         skipped++;
         return new StreamingCondition.Skipping();
      }
      else
      {
         return new StreamingCondition.Continuing(state.Next);
      }
   }

   public override string ToString() => $"skip({count})";
}