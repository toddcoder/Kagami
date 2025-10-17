using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingAssoc(IObject target) : StreamingAction
{
   public override StreamingCondition Execute(StreamingState state)
   {
      if (state.Next is ICollection collection)
      {
         var first = collection.GetIterator(false).First();
         if (first is Some some && some.Value.IsEqualTo(target))
         {
            return new StreamingCondition.Terminated(some);
         }
         else
         {
            return new StreamingCondition.Skipping();
         }
      }
      else
      {
         return new StreamingCondition.Skipping();
      }
   }

   public override string ToString() => $"assoc({target.AsString})";
}