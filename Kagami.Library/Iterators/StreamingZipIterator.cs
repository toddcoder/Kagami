using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Iterators;

public class StreamingZipIterator(IIterator iterator) : StreamingAction
{
   public override StreamingCondition Execute(StreamingState state)
   {
      var _next = iterator.Next();
      if (_next is (true, var next))
      {
         List<IObject> result = [state.Next, next];
         var reverted = state.CollectionClass.Revert(result, nil);

         return new StreamingCondition.Continuing(reverted);
      }
      else
      {
         return new StreamingCondition.Finished();
      }
   }

   public override string ToString() => $"zip({((IObject)iterator.Collection).ClassName}";
}