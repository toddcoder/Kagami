using System.Numerics;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public struct LongRange : IObject, ICollection
{
   private Long start;
   private IObject startObj;
   private Long stop;
   private IObject stopObj;
   private bool inclusive;
   private BigInteger increment;
   private Func<Long, Long> next;
   private Func<Long, IObject, bool> compare;
   private Maybe<IIterator> _currentIterator = nil;

   public LongRange(Long start, Long stop, bool inclusive, BigInteger increment) : this()
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
               current = (Long)current.Successor;
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
               current = (Long)current.Predecessor;
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

   public LongRange(LongRange kRange, Long increment) : this(kRange.start, kRange.stop, kRange.inclusive, increment.Value)
   {
   }

   public Long Start => start;

   public IObject StartObj => startObj;

   public Long Stop => stop;

   public IObject StopObj => stopObj;

   public bool Inclusive => inclusive;

   public BigInteger Increment => increment;

   public Func<Long, IObject, bool> Compare => compare;

   public Func<Long, Long> NextValue => next;

   public string ClassName => "LongRange";

   private static string str(IObject obj, bool asString) => asString ? obj.AsString : obj.Image;

   private string startImage(bool asString) => str(startObj, asString);

   private string stopImage(bool asString) => str(stopObj, asString);

   private string inclusiveImage() => inclusive ? "" : "<";

   private string incrementImage() => $"{(increment >= 0 ? "+" : "-")} {BigInteger.Abs(increment)}";

   public string AsString => $"{startImage(true)} ..{inclusiveImage()} {stopImage(true)} {incrementImage()}";

   public string Image => $"{startImage(false)} ..{inclusiveImage()} {stopImage(false)} {incrementImage()}";

   public int Hash => HashCode.Combine(startObj, stopObj, increment);

   public bool IsEqualTo(IObject obj)
   {
      return obj is LongRange r && startObj.IsEqualTo(r.startObj) && stopObj.IsEqualTo(r.stopObj) && increment == r.increment &&
         inclusive == r.inclusive;
   }

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => false;

   public bool IsTrue => list(this).Any();

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);

   public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new LongRangeIterator(this);

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
      return new LongRange(new Long(start.Value * count), new Long(stop.Value * count), inclusive, increment * count);
   }

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IObject One() => this;

   public IObject Copy() => new LongRange(start, stop, inclusive, increment);

   public IIterator Following(IObject following) => new MultiIterator(this, following);

   public IObject Add(BigInteger increment) => new LongRange(this, increment);

   public IObject Subtract(BigInteger increment) => new LongRange(this, -increment);

   public LongRange Reverse() => new(stop, start, true, -increment);

   public IObject Concatenate(LongRange otherRange)
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

   public IObject Max() => stopObj is Infinity ? stopObj : new LongRangeIterator(this).Max();
}