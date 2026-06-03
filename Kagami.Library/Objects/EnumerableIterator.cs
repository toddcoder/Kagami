using Core.Monads;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class EnumerableIterator : Iterator
{
   protected IEnumerable<IObject> enumerable;
   protected Maybe<IEnumerator<IObject>> _enumerator = nil;

   public EnumerableIterator(IEnumerable<IObject> enumerable) : base(new KArray([]))
   {
      this.enumerable = enumerable;
   }

   public override bool IsLazy => true;

   public override Maybe<IObject> Next()
   {
      (var enumerator, _enumerator) = _enumerator.Create(() => enumerable.GetEnumerator());
      if (enumerator.MoveNext())
      {
         return enumerator.Current.Some();
      }
      else
      {
         return nil;
      }
   }

   public override Maybe<IObject> Peek()
   {
      if (_enumerator is (true, var enumerator))
      {
         return enumerator.Current.Some();
      }
      else
      {
         return nil;
      }
   }

   public override IObject Reset()
   {
      _enumerator = nil;
      return this;
   }

   public override IEnumerable<IObject> List()
   {
      index = 0;
      while (Next() is (true, var item))
      {
         yield return item;

         if (index++ % 1000 == 0 && Machine.Current.Context.Cancelled())
         {
            yield break;
         }
      }
   }
}