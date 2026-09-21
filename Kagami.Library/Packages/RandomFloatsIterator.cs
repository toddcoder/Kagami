using Core.Monads;
using Kagami.Library.Objects;

namespace Kagami.Library.Packages;

public class RandomFloatsIterator : RandomIntsIterator
{
   public RandomFloatsIterator(Random random) : base(random)
   {
   }

   public override Maybe<IObject> Next() => Float.FloatObject((float)random.NextDouble()).Some();
}