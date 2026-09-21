using Core.Monads;
using Kagami.Library.Objects;

namespace Kagami.Library.Packages;

public class RandomIntsIterator : LazyIterator
{
   protected Random random;

   public RandomIntsIterator(Random random) : base(KArray.Empty)
   {
      this.random = random;
   }

   public override Maybe<IObject> Next() => Int.IntObject(random.Next()).Some();
}