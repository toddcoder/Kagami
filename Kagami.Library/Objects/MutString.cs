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

namespace Kagami.Library.Objects;

public class MutString : IObject, IComparable<MutString>, IEquatable<MutString>, IFormattable, IComparable, ISliceable,
   IRangeItem, ITextFinding, IMutableCollection
{
   public static implicit operator MutString(string source) => new(source);

   public static implicit operator MutString(KString source) => new(source.AsString);

   public static implicit operator string(MutString source) => source.AsString;

   public static implicit operator KString(MutString source) => source.AsString;

   protected StringBuilder mutable;

   public MutString(string mutable) => this.mutable = new StringBuilder(mutable);

   public string ClassName => "MutString";

   public string AsString => mutable.ToString();

   public string Image => $"m\"{AsString.Replace("\"", @"\""").Replace("\t", @"\t")}\"";

   public int Hash => mutable.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is MutString ms && mutable == ms.mutable || obj is KString s && AsString == s.AsString;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => mutable.Length > 0;

   public int CompareTo(MutString? other) => Compare(other!);

   public bool Equals(MutString? other) => AsString == other!.AsString;

   public KString Format(string format) => AsString.FormatAs(format);

   public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

   public Maybe<IObject> Next(int index)
   {
      var self = this;
      return maybe<IObject>() & index < mutable.Length & (() => KChar.CharObject(self.mutable[index]));
   }

   public Maybe<IObject> Peek(int index) => Next(index);

   Int ICollection.Length => Length;

   public Slice Slice(ICollection collection) => new(this, collection.GetIterator(false).List().ToArray());

   public Maybe<IObject> Get(IObject index)
   {
      var intIndex = wrapIndex(((Int)index).Value, mutable.Length);
      return Next(intIndex);
   }

   public KChar this[int index]
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

   public KBoolean In(IObject item)
   {
      return AsString.Contains(item.AsString) || item is KChar c && AsString.IndexOf(c.Value) > 0;
   }

   public KBoolean NotIn(IObject item) => !In(item).Value;

   public IObject Times(int count) => new MutString(AsString.Repeat(count));

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public int CompareTo(object? obj) => AsString.CompareTo(obj!.ToString());

   public int Compare(IObject obj) => AsString.CompareTo(obj.AsString);

   public IObject Object => this;

   public KBoolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

   public KBoolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

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

   public KRange Range() => new(this, (MutString)"z".Repeat(mutable.Length), true);

   public IObject Find(string input, int startIndex, bool reverse) => find(AsString, input, startIndex, reverse);

   public KTuple FindAll(string input) => findAll(AsString, input);

   public KString Replace(string input, string replacement, bool reverse)
   {
      var result = replace(AsString, input, replacement, reverse);
      mutable.Clear();
      mutable.Append(result.Value);
      return result;
   }

   public KString Replace(string input, Lambda lambda, bool reverse)
   {
      var result = replace(AsString, input, lambda, reverse);
      mutable.Clear();
      mutable.Append(result.Value);
      return result;
   }

   public KString ReplaceAll(string input, string replacement)
   {
      var result = replaceAll(AsString, input, replacement);
      mutable.Clear();
      mutable.Append(result.Value);
      return result;
   }

   public KString ReplaceAll(string input, Lambda lambda)
   {
      var result = replaceAll(AsString, input, lambda);
      mutable.Clear();
      mutable.Append(result.Value);
      return result;
   }

   public KTuple Split(string input) => split(AsString, input);

   public KTuple Partition(string input, bool reverse) => partition(AsString, input, reverse);

   public Int Count(string input) => count(AsString, input);

   public Int Count(string input, Lambda lambda) => count(AsString, input, lambda);

   public MutString Append(IObject obj)
   {
      mutable.Append(obj.AsString);
      return this;
   }

   public IObject Remove(IObject obj)
   {
      if (mutable.ToString().Find(obj.AsString) is (true, var index))
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

   public KBoolean IsEmpty => mutable.Length == 0;

   public IObject Assign(SkipTake skipTake, IEnumerable<IObject> values)
   {
      var array = mutable.ToString().ToCharArray();
      var left = array.Skip(skipTake.Skip);
      var right = left.Skip(skipTake.Skip + skipTake.Take);

      var newMutable = new StringBuilder();
      newMutable.Append(left);
      newMutable.Append(values.Cast<KChar>());
      newMutable.Append(right);

      mutable = newMutable;
      return this;
   }

   public MutString Fill(char ch, int count)
   {
      mutable.Clear();
      mutable.Append(ch.Repeat(count));

      return this;
   }

   IObject IMutableCollection.Append(IObject obj) => Append(obj);

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);
}