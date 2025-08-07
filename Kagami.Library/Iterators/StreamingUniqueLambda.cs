using Core.Collections;
using Core.Enumerables;
using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingUniqueLambda(Lambda lambda) : StreamingAction
{
   protected Set<IObject> alreadyEmitted = [];

   public override StreamingCondition Execute(StreamingState state)
   {
      if (!alreadyEmitted.AtLeastOne(i => lambda.Invoke(i, state.Next).IsTrue))
      {
         alreadyEmitted.Add(state.Next);
         return new StreamingCondition.Continuing(state.Next);
      }
      else
      {
         return new StreamingCondition.Skipping();
      }
   }

   public override string ToString() => $"unique({lambda.Image})";
}