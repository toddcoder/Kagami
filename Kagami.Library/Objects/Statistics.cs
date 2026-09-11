using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public struct Statistics(IIterator iterator) : IObject
{
   private double sum = 0.0;
   private int count = 0;
   private double average = 0.0;
   private Maybe<IObject> _min = nil;
   private Maybe<IObject> _max = nil;

   public Float Sum => sum;

   public Int Count => count;

   public Float Average => average;

   public Maybe<IObject> Min => _min;

   public Maybe<IObject> Max => _max;

   public IObject Evaluate()
   {
      try
      {
         while (iterator.Next() is (true, var value))
         {
            if (value is INumeric numeric)
            {
               sum += numeric.AsDouble();
               count += 1;
               if (_min is (true, var min) && value is IComparable minuComparable)
               {
                  _min = (minuComparable.CompareTo(min) < 0 ? value : min).Some();
               }
               else
               {
                  _min = value.Some();
               }

               if (_max is (true, var max) && value is IComparable maxComparable)
               {
                  _max = (maxComparable.CompareTo(max) > 0 ? value : max).Some();
               }
               else
               {
                  _max = value.Some();
               }
            }
            else
            {
               return new Failure($"Value {value.Image} is not numeric");
            }
         }

         if (count > 0)
         {
            average = sum / count;
         }
         else
         {
            return new Failure("Can't divide by zero");
         }

         return Success.Object(this);
      }
      catch (Exception exception)
      {
         return new Failure(exception.Message);
      }
   }

   public string ClassName => "Statistics";

   public string AsString =>
      $"Statistics(sum: {sum}, count: {count}, average: {average}, min: {_min.Map(i => i.AsString) | "?"}, max: {_max.Map(i => i.AsString) | "?"})";

   public string Image => AsString;

   public int Hash => HashCode.Combine(sum, count, average, _min, _max);

   public bool IsEqualTo(IObject obj) => obj is Statistics statistics && sum == statistics.sum && count == statistics.count &&
      average == statistics.average && _min == statistics._min && _max == statistics._max;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, (x, y) => x.IsEqualTo(y), bindings);

   public bool IsTrue => count > 0;

   public Guid Id { get; init; } = Guid.NewGuid();
}