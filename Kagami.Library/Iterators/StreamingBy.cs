using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Iterators;

public class StreamingBy(int count) : StreamingAction
{
   private List<IObject> accumulated = [];

   public override StreamingCondition Execute(StreamingState state)
   {
      accumulated.Add(state.Next);
      if (accumulated.Count == count)
      {
         var result = state.CollectionClass.Revert(accumulated, nil);
         accumulated.Clear();

         return new StreamingCondition.Continuing(result);
      }
      else
      {
         return new StreamingCondition.Skipping();
      }
   }

   public override string ToString() => $"by({count})";
}