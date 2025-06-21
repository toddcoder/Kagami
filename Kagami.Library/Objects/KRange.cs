using Core.Collections;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;

namespace Kagami.Library.Objects;

public readonly struct KRange : IObject, ICollection
{
   private readonly IRangeItem start;
   private readonly IObject startObj;
   private readonly IObjectCompare stop;
   private readonly IObject stopObj;
   private readonly bool inclusive;
   private readonly int increment;
   private readonly Func<IRangeItem, IRangeItem> next;
   private readonly Func<IRangeItem, IObject, bool> compare;

   public KRange(IRangeItem start, IObjectCompare stop, bool inclusive, int increment = 1) : this()
   {
      this.start = start;
      startObj = this.start.Object;
      this.stop = stop;
      stopObj = this.stop.Object;
      this.inclusive = inclusive;
      this.increment = increment;
      if (this.increment > 0)
      {
         next = i =>
         {
            var current = i;
            for (var j = 0; j < increment; j++)
            {
               current = current.Successor;
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
               current = current.Predecessor;
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

   public KRange(KRange kRange, Int increment) : this(kRange.start, kRange.stop, kRange.inclusive, increment.Value)
   {
   }

   public IRangeItem Start => start;

   public IObject StartObj => startObj;

   public IObjectCompare Stop => stop;

   public IObject StopObj => stopObj;

   public bool Inclusive => inclusive;

   public int Increment => increment;

   public Func<IRangeItem, IObject, bool> Compare => compare;

   public Func<IRangeItem, IRangeItem> NextValue => next;

   public string ClassName => "Range";

   private static string str(IObject obj, bool asString) => asString ? obj.AsString : obj.Image;

   private string startImage(bool asString) => str(startObj, asString);

   private string stopImage(bool asString) => str(stopObj, asString);

   private string inclusiveImage() => inclusive ? "" : "<";

   private string incrementImage() => $"{(increment >= 0 ? "+" : "-")} {Math.Abs(increment)}";

   public string AsString => $"{startImage(true)} ..{inclusiveImage()} {stopImage(true)} {incrementImage()}";

   public string Image => $"{startImage(false)} ..{inclusiveImage()} {stopImage(false)} {incrementImage()}";

   public int Hash => (startObj.Hash + stopObj.Hash + increment.GetHashCode()).GetHashCode();

   public bool IsEqualTo(IObject obj)
   {
      return obj is KRange r && startObj.IsEqualTo(r.startObj) && stopObj.IsEqualTo(r.stopObj) && increment == r.increment &&
         inclusive == r.inclusive;
   }

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => false;

   public bool IsTrue => list(this).Any();

   public Guid Id { get; init; } = Guid.NewGuid();

   public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(new KArray(list(this))) : new RangeIterator(this);

   public Maybe<IObject> Next(int index) => index == 0 ? GetIterator(false).ToArray() : nil;

   public Maybe<IObject> Peek(int index) => nil;

   public Int Length => list(this).Count();

   public bool ExpandForArray => true;

   public KBoolean In(IObject comparisand)
   {
      if (comparisand is IObjectCompare oc && startObj is IObjectCompare left)
      {
         if (left.Compare(comparisand) > 0)
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

   public KBoolean NotIn(IObject comparisand) => !In(comparisand).IsTrue;

   public IObject Times(int count)
   {
      if (start is Int iStart && stop is Int iStop)
      {
         return new KRange((Int)(iStart.Value * count), (Int)(iStop.Value * count), inclusive, increment * count);
      }
      else
      {
         return new KRange(start, stop, inclusive, increment * 4);
      }
   }

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator()
   {
      var iterator = GetIterator(false);
      var array = new KArray(iterator.List());

      return new IndexedIterator(array);
   }

   public IObject Add(int increment) => new KRange(this, increment);

   public IObject Subtract(int increment) => new KRange(this, -increment);

   public KRange Reverse() => new((IRangeItem)stop, start, true, -increment);

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);

   public IObject Concatenate(KRange otherRange)
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

   public IObject Max() => stopObj is Infinity ? stopObj : new RangeIterator(this).Max();
}