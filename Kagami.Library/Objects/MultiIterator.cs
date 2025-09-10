using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class MultiIterator(ICollection collection, IObject following) : Iterator(collection)
{
   protected Maybe<IIterator> _followingIterator = nil;

   public override Maybe<IObject> Next()
   {
      if (_followingIterator is (true, var followingIterator))
      {
         var _next = followingIterator.Next();
         if (_next)
         {
            return _next;
         }
         else
         {
            return nil;
         }
      }
      else
      {
         var _next = base.Next();
         if (_next)
         {
            return _next;
         }
         else
         {
            _followingIterator = following switch
            {
               ICollection followingCollection => followingCollection.GetIterator(false).Some(),
               IIterator newIterator => newIterator.Some(),
               _ => nil
            };
            if (_followingIterator is (true, var newFollowingIterator))
            {
               return newFollowingIterator.Next();
            }
            else
            {
               return nil;
            }
         }
      }
   }
}