using System;
using System.Linq;
using System.Text;
using Core.Collections;
using Core.Monads;
using Core.Numbers;
using Core.Objects;
using Core.Strings;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Objects.TextFindingFunctions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Operations.OperationFunctions;

namespace Kagami.Library.Objects
{
	public class MutString : IObject, IComparable<MutString>, IEquatable<MutString>, IFormattable, IComparable, ISliceable,
		IRangeItem, ITextFinding, IMutableCollection
	{
		public static implicit operator MutString(string source) => new MutString(source);

		public static implicit operator MutString(String source) => new MutString(source.AsString);

		public static implicit operator string(MutString source) => source.AsString;

		public static implicit operator String(MutString source) => source.AsString;

		StringBuilder mutable;

		public MutString(string mutable) => this.mutable = new StringBuilder(mutable);

		public string ClassName => "MutString";

		public string AsString => mutable.ToString();

		public string Image => $"m\"{AsString.Replace("\"", @"\""").Replace("\t", @"\t")}\"";

		public int Hash => mutable.GetHashCode();

		public bool IsEqualTo(IObject obj) => obj is MutString ms && mutable == ms.mutable || obj is String s && AsString == s.AsString;

		public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

		public bool IsTrue => mutable.Length > 0;

		public int CompareTo(MutString other) => Compare(other);

		public bool Equals(MutString other) => AsString == other.AsString;

		public String Format(string format) => AsString.FormatAs(format);

		public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

		public IMaybe<IObject> Next(int index)
		{
			var self = this;
			return maybe(index < mutable.Length, () => Char.CharObject(self.mutable[index]));
		}

		public IMaybe<IObject> Peek(int index) => Next(index);

		Int ICollection.Length => Length;

		public Slice Slice(ICollection collection) => new Slice(this, collection.GetIterator(false).List().ToArray());

		public IMaybe<IObject> Get(IObject index)
		{
			var intIndex = wrapIndex(((Int)index).Value, mutable.Length);
			return Next(intIndex);
		}

		public Char this[int index]
		{
			get
			{
				index = wrapIndex(index, mutable.Length);
				return mutable[index];
			}
			set
			{
				index = wrapIndex(index, mutable.Length);
				mutable[index] = value.Value;
			}
		}

		public IObject Set(IObject index, IObject value)
		{
			var intIndex = wrapIndex(((Int)index).Value, mutable.Length);
			mutable[intIndex] = value.AsString[0];

			return this;
		}

		public bool ExpandForArray => false;

		public int Length => mutable.Length;

		public Boolean In(IObject item)
		{
			return AsString.Contains(item.AsString) || item is Char c && AsString.IndexOf(c.Value) > 0;
		}

		public Boolean NotIn(IObject item) => !In(item).Value;

		public IObject Times(int count) => new MutString(AsString.Repeat(count));

		public String MakeString(string connector) => makeString(this, connector);

		public IIterator GetIndexedIterator() => new IndexedIterator(this);

		public int CompareTo(object obj) => AsString.CompareTo(obj.ToString());

		public int Compare(IObject obj) => AsString.CompareTo(obj.AsString);

		public IObject Object => this;

		public Boolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

		public Boolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

		public IRangeItem Successor
		{
			get
			{
				mutable = new StringBuilder(AsString.Succ());
				return this;
			}
		}

		public IRangeItem Predecessor
		{
			get
			{
				mutable = new StringBuilder(AsString.Pred());
				return this;
			}
		}

		public Range Range() => new Range(this, (MutString)"z".Repeat(mutable.Length), true);

		public IObject Find(string input, int startIndex, bool reverse) => find(AsString, input, startIndex, reverse);

		public Tuple FindAll(string input) => findAll(AsString, input);

		public String Replace(string input, string replacement, bool reverse)
		{
			var result = replace(AsString, input, replacement, reverse);
			mutable.Clear();
			mutable.Append(result.Value);
			return result;
		}

		public String Replace(string input, Lambda lambda, bool reverse)
		{
			var result = replace(AsString, input, lambda, reverse);
			mutable.Clear();
			mutable.Append(result.Value);
			return result;
		}

		public String ReplaceAll(string input, string replacement)
		{
			var result = replaceAll(AsString, input, replacement);
			mutable.Clear();
			mutable.Append(result.Value);
			return result;
		}

		public String ReplaceAll(string input, Lambda lambda)
		{
			var result = replaceAll(AsString, input, lambda);
			mutable.Clear();
			mutable.Append(result.Value);
			return result;
		}

		public Tuple Split(string input) => split(AsString, input);

		public Tuple Partition(string input, bool reverse) => partition(AsString, input, reverse);

		public Int Count(string input) => count(AsString, input);

		public Int Count(string input, Lambda lambda) => count(AsString, input, lambda);

		public MutString Append(IObject obj)
		{
			mutable.Append(obj.AsString);
			return this;
		}

		public IObject Remove(IObject obj)
		{
			if (mutable.ToString().Find(obj.AsString).If(out var index))
			{
				mutable.Remove(index, obj.AsString.Length);
			}

			return this;
		}

		public IObject RemoveAt(int index)
		{
			if (index.Between(0).Until(mutable.Length))
			{
				mutable.Remove(index, 1);
			}

			return this;
		}

		public IObject RemoveAll(IObject obj)
		{
			var asString = obj.AsString;
			var length = asString.Length;
			foreach (var index in mutable.ToString().FindAll(asString).Reverse())
			{
				mutable.Remove(index, length);
			}

			return this;
		}

		public IObject InsertAt(int index, IObject obj)
		{
			mutable.Insert(index, obj.AsString);
			return this;
		}

		public Boolean IsEmpty => mutable.Length == 0;

		public IObject Assign(IObject indexes, IObject values)
		{
			if (getIterator(indexes, false).If(out var indexesIterator) && getIterator(values, false).If(out var valuesIterator))
			{
				while (indexesIterator.Next().If(out var index))
				{
					if (valuesIterator.Next().If(out var value))
					{
						if (index is Int i && i.Value.Between(0).Until(mutable.Length) && value is Char ch)
						{
							mutable[i.Value] = ch.Value;
						}
					}
					else
					{
						break;
					}
				}
			}

			return this;
		}

		public MutString Fill(char ch, int count)
		{
			mutable.Clear();
			mutable.Append(ch.Repeat(count));

			return this;
		}

		IObject IMutableCollection.Append(IObject obj) => Append(obj);
	}
}