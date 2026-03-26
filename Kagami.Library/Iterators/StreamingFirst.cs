namespace Kagami.Library.Iterators;

public class StreamingFirst : StreamingAction
{
   public override StreamingCondition Execute(StreamingState state)
   {
      return new StreamingCondition.Terminated(state.Next);
   }

   public override string ToString() => "first()";
}