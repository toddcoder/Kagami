using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class RangeIterator : LazyIterator
{
   protected KRange kRange;
   protected IRangeItem current;
   protected IObject stop;

   public RangeIterator(KRange kRange) : base(kRange)
   {
      this.kRange = kRange;
      current = kRange.Start;
      stop = kRange.StopObj;
   }

   public override Maybe<IObject> Next() => kRange.Next(index++);

   public override Maybe<IObject> Peek() => maybe<IObject>() & kRange.Compare(current, stop) & (() => current.Object);

   public override IEnumerable<IObject> List()
   {
      while (kRange.Compare(current, stop))
      {
         yield return current.Object;

         current = kRange.NextValue(current);
      }
   }
}