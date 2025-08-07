namespace Kagami.Library.Iterators;

public abstract class StreamingAction
{
   public abstract StreamingCondition Execute(StreamingState state);
}