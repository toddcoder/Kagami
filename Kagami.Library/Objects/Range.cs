using System;
using System.Linq;
using Core.Collections;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;

namespace Kagami.Library.Objects
{
	public struct Range : IObject, ICollection
	{
		IRangeItem start;
		IObject startObj;
		IObjectCompare stop;
		IObject stopObj;
		bool inclusive;
		int increment;
		Func<IRangeItem, IRangeItem> next;
		Func<IRangeItem, IObject, bool> compare;

		public Range(IRangeItem start, IObjectCompare stop, bool inclusive, int increment = 1) : this()
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

		public Range(Range range, Int increment) : this(range.start, range.stop, range.inclusive, increment.Value) { }

		public IRangeItem Start => start;

		public IObject StartObj => startObj;

		public IObjectCompare Stop => stop;

		public IObject StopObj => stopObj;

		public bool Inclusive => inclusive;

		public int Increment => increment;

		public Func<IRangeItem, IObject, bool> Compare => compare;

		public Func<IRangeItem, IRangeItem> NextValue => next;

		public string ClassName => "Range";

		public string AsString => $"{startObj.AsString} ... {stopObj.AsString} {(increment >= 0 ? "+" : "-")} {Math.Abs(increment)}";

		public string Image => $"{startObj.Image} ... {stopObj.Image} {(increment >= 0 ? "+" : "-")} {Math.Abs(increment)}";

		public int Hash => (startObj.Hash + stopObj.Hash + increment.GetHashCode()).GetHashCode();

		public bool IsEqualTo(IObject obj)
		{
			return obj is Range r && startObj.IsEqualTo(r.startObj) && stopObj.IsEqualTo(r.stopObj) && increment == r.increment;
		}

		public bool Match(IObject comparisand, Hash<string, IObject> bindings) => false;

		public bool IsTrue => list(this).Any();

		public IIterator GetIterator(bool lazy) => lazy ? (IIterator)new LazyIterator(new Array(list(this))) : new RangeIterator(this);

		public IMaybe<IObject> Next(int index)
		{
			if (index == 0)
			{
				return GetIterator(false).ToArray().Some<IObject>();
			}
			else
			{
				return none<IObject>();
			}
		}

		public IMaybe<IObject> Peek(int index) => none<IObject>();

		public Int Length => list(this).Count();

		public bool ExpandForArray => true;

		public Boolean In(IObject comparisand)
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

		public Boolean NotIn(IObject comparisand) => !In(comparisand).IsTrue;

		public IObject Times(int count) => this;

		public String MakeString(string connector) => makeString(this, connector);

		public IIterator GetIndexedIterator()
		{
			var iterator = GetIterator(false);
			var array = new Array(iterator.List());
			return new IndexedIterator(array);
		}

		public IObject Add(int increment) => new Range(this, increment);

		public IObject Subtract(int increment) => new Range(this, -increment);

		public Range Reverse() => new Range((IRangeItem)stop, start, true, -increment);
	}
}