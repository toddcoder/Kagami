namespace Kagami.Library.Iterators;

public class StreamingTake(int count) : StreamingAction
{
   protected int taken;

   public override StreamingCondition Execute(StreamingState state)
   {
      if (taken < count)
      {
         taken++;
         return new StreamingCondition.Continuing(state.Next);
      }
      else
      {
         return new StreamingCondition.Finished();
      }
   }

   public override string ToString() => $"take({count})";
}