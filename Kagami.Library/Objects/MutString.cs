using System;
using System.Linq;
using System.Text;
using Core.Collections;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Objects.TextFindingFunctions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;

namespace Kagami.Library.Objects
{
	public class MutString : IObject, IComparable<MutString>, IEquatable<MutString>, IFormattable, ICollection, IComparable, ISliceable,
		IRangeItem, ITextFinding
	{
		public static implicit operator MutString(string source) => new MutString(source);

		public static implicit operator MutString(String source) => new MutString(source.AsString);

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
			return when(index < mutable.Length, () => Char.CharObject(self.mutable[index]));
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

		public String Replace(string input, string replacement, bool reverse) => replace(AsString, input, replacement, reverse);

		public String Replace(string input, Lambda lambda, bool reverse) => replace(AsString, input, lambda, reverse);

		public String ReplaceAll(string input, string replacement) => replaceAll(AsString, input, replacement);

		public String ReplaceAll(string input, Lambda lambda) => replaceAll(AsString, input, lambda);

		public Tuple Split(string input) => split(AsString, input);

		public Tuple Partition(string input, bool reverse) => partition(AsString, input, reverse);

		public Int Count(string input) => count(AsString, input);

		public Int Count(string input, Lambda lambda) => count(AsString, input, lambda);

		public MutString Append(IObject obj)
		{
			mutable.Append(obj.AsString);
			return this;
		}

		public MutString Fill(char ch, int count)
		{
			mutable.Clear();
			mutable.Append(ch.Repeat(count));

			return this;
		}
	}
}