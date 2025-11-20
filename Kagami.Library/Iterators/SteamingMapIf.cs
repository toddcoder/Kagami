using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class SteamingMapIf(Lambda lambda) : StreamingAction
{
   public override StreamingCondition Execute(StreamingState state)
   {
      var result = lambda.Invoke(state.Next);
      return result switch
      {
         Some some => new StreamingCondition.Continuing(some.Value),
         Success success => new StreamingCondition.Continuing(success.Value),
         _ => new StreamingCondition.Continuing(state.Next)
      };
   }

   public override string ToString() => $"mapIf({lambda.Image})";
}