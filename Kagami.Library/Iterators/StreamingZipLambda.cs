using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingZipLambda(ICollection collection, Lambda lambda) : StreamingZipLambdaIterator(collection.GetIterator(false), lambda)
{
   protected Lambda lambda = lambda;

   public override StreamingCondition Execute(StreamingState state)
   {
      var _next = iterator.Next();
      if (_next is (true, var next))
      {
         var result = lambda.Invoke(state.Next, next);
         return new StreamingCondition.Continuing(result);
      }
      else
      {
         return new StreamingCondition.Finished();
      }
   }

   public override string ToString() => $"zip({((IObject)collection).ClassName}, {lambda.Image})";
}