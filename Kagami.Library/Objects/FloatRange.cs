using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public struct FloatRange : IObject, ICollection
{
   private Float start;
   private IObject startObj;
   private Float stop;
   private IObject stopObj;
   private bool inclusive;
   private double increment;
   private Func<Float, Float> next;
   private Func<Float, IObject, bool> compare;
   private Maybe<IIterator> _currentIterator = nil;

   public FloatRange(Float start, Float stop, bool inclusive, double increment = 1)
   {
      this.start = start;
      this.stop = stop;
      this.inclusive = inclusive;
      this.increment = increment;

      startObj = start;
      stopObj = stop;

      if (this.increment > 0)
      {
         next = i =>
         {
            var current = i;
            for (var j = 0; j < increment; j++)
            {
               current = (Float)current.Successor;
            }

            return current;
         };
         if (inclusive)
         {
            compare = (i, o) => i.Compare(o) <= 0;
         }
         else
         {
            compare = (i, o) => i.Compare(o) < 0;
         }
      }
      else
      {
         next = i =>
         {
            var current = i;
            for (var j = 0; j < -increment; j++)
            {
               current = (Float)current.Predecessor;
            }

            return current;
         };
         if (inclusive)
         {
            compare = (i, o) => i.Compare(o) >= 0;
         }
         else
         {
            compare = (i, o) => i.Compare(o) > 0;
         }
      }
   }

   public FloatRange(FloatRange range, double increment) : this(range.start, range.stop, range.inclusive, increment)
   {
   }

   public Float Start => start;

   public IObject StartObj => startObj;

   public Float Stop => stop;

   public IObject StopObj => stopObj;

   public bool Inclusive => inclusive;

   public double Increment => increment;

   public Func<Float, IObject, bool> Compare => compare;

   public Func<Float, Float> NextValue => next;

   public string ClassName => "FloatRange";

   private static string str(IObject obj, bool asString) => asString ? obj.AsString : obj.Image;

   private string startImage(bool asString) => str(startObj, asString);

   private string stopImage(bool asString) => str(stopObj, asString);

   private string inclusiveImage() => inclusive ? "" : "<";

   private string incrementImage() => $"{(increment >= 0 ? "+" : "-")} {Math.Abs(increment)}";

   public string AsString => $"{startImage(true)} ..{inclusiveImage()} {stopImage(true)} {incrementImage()}";

   public string Image => $"{startImage(false)} ..{inclusiveImage()} {stopImage(false)} {incrementImage()}";

   public int Hash => HashCode.Combine(startObj, stopObj, increment);

   public bool IsEqualTo(IObject obj) => obj is FloatRange r && startObj.IsEqualTo(r.startObj) && stopObj.IsEqualTo(r.stopObj) &&
      increment == r.increment && inclusive == r.inclusive;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => list(this).Any();

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);

   public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new FloatRangeIterator(this);

   public Maybe<IObject> Next(int index)
   {
      if (index == 0)
      {
         _currentIterator = GetIterator(false).Some();
      }

      if (_currentIterator is (true, var currentIterator))
      {
         var _next = currentIterator.Next();
         if (!_next)
         {
            _currentIterator = nil;
         }

         return _next;
      }
      else
      {
         return nil;
      }
   }

   public Maybe<IObject> Peek(int index) => nil;

   public Int Length => list(this).Count();

   public bool ExpandForArray => true;

   public KBoolean In(IObject item)
   {
      if (item is IObjectCompare oc && startObj is IObjectCompare left)
      {
         if (left.Compare(item) > 0)
         {
            return false;
         }
         else if (inclusive)
         {
            return oc.Compare(stopObj) <= 0;
         }
         else
         {
            return oc.Compare(stopObj) < 0;
         }
      }
      else
      {
         return false;
      }
   }

   public KBoolean NotIn(IObject item) => !In(item).IsTrue;

   public IObject Times(int count)
   {
      return new FloatRange(new Float(start.Value * count), new Float(stop.Value * count), inclusive, increment * count);
   }

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IObject One() => this;

   public IObject Copy() => new FloatRange(start, stop, inclusive, increment);

   public IIterator Following(IObject following) => new MultiIterator(this, following);

   public IObject Add(double increment) => new FloatRange(this, increment);

   public IObject Subtract(double increment) => new FloatRange(this, -increment);

   public FloatRange Reverse() => new(stop, start, true, -increment);

   public IObject Concatenate(FloatRange otherRange)
   {
      var iterator = GetIterator(false);
      var list = iterator.List();
      var sequence = new Sequence(list);

      var otherIterator = otherRange.GetIterator(false);
      var otherList = otherIterator.List();
      foreach (var obj in otherList)
      {
         sequence.Add(obj);
      }

      return sequence;
   }

   public IObject Max() => stopObj is Infinity ? stopObj : new FloatRangeIterator(this).Max();
}