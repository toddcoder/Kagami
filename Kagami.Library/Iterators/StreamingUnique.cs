using Core.Collections;
using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingUnique : StreamingAction
{
   protected Set<IObject> alreadyEmitted = [];

   public override StreamingCondition Execute(StreamingState state)
   {
      if (alreadyEmitted.Contains(state.Next))
      {
         return new StreamingCondition.Skipping();
      }
      else
      {
         alreadyEmitted.Add(state.Next);
         return new StreamingCondition.Continuing(state.Next);
      }
   }

   public override string ToString() => "unique()";
}