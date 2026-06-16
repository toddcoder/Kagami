using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class Cover(ICollection collection) : Iterator(collection)
{
   protected Maybe<Lambda> _first = nil;
   protected Maybe<Lambda> _middle = nil;
   protected Maybe<Lambda> _last = nil;
   protected bool firstExecuted;

   public override string ClassName => "Cover";

   public override IObject First(Lambda lambda)
   {
      _first = lambda;
      return this;
   }

   public IObject Middle(Lambda lambda)
   {
      _middle = lambda;
      return this;
   }

   public override IObject Last(Lambda lambda)
   {
      _last = lambda;
      return this;
   }

   public override Maybe<IObject> Next()
   {
      if (base.Next() is (true, var next))
      {
         if (!firstExecuted && _first is (true, var firstLambda))
         {
            firstLambda.Invoke(next);
            firstExecuted = true;
            return next.Some();
         }

         if (index == collection.Length.Value && _last is (true, var lastLambda))
         {
            lastLambda.Invoke(next);
            return next.Some();
         }

         if (_middle is (true, var middleLambda))
         {
            middleLambda.Invoke(next);
         }

         return next.Some();
      }
      else
      {
         return nil;
      }
   }
}