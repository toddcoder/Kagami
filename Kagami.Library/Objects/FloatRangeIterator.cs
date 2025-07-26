using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class FloatRangeIterator : Iterator
{
   protected FloatRange range;
   protected double current;
   protected double stop;

   public FloatRangeIterator(FloatRange range) : base(range)
   {
      this.range = range;
      current = range.Start.Value;
      stop = range.Stop.Value;
   }

   public override Maybe<IObject> Next()
   {
      if (range.Compare(current, (Float)stop))
      {
         var result = current;
         current = range.NextValue(current).Value;

         return Float.FloatObject(result).Some();
      }
      else
      {
         return nil;
      }
   }

   public override Maybe<IObject> Peek() => maybe<IObject>() & range.Compare(current, (Float)stop) & (() => Float.FloatObject(current));

   public override IEnumerable<IObject> List()
   {
      while (range.Compare(current, Float.FloatObject(stop)))
      {
         yield return Float.FloatObject(current);

         current = range.NextValue(current).Value;
      }
   }
}