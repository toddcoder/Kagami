using System.Numerics;
using Core.Monads;
using Kagami.Library.Objects;

namespace Kagami.Library.Packages;

public class PrimeIterator : LazyIterator
{
   protected BigInteger current = new(2);
   protected BigInteger two = new(2);

   public PrimeIterator() : base(KArray.Empty)
   {
   }

   public override Maybe<IObject> Next()
   {
      var next = Long.LongObject(current);
      current = nextPrime();

      return next.Some();
   }

   protected BigInteger nextPrime()
   {
      if (current.IsEven)
      {
         current += 1;
      }
      else
      {
         current += 2;
      }

      var asLong = new Long(current);
      if (asLong.IsPrime.Value)
      {
         return current;
      }
      else
      {
         return nextPrime();
      }
   }
}