using Core.Collections;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using Core.Objects;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Formatter(LazyString lazyString, KArray array) : IObject, IComparable<KString>, IEquatable<KString>, IFormattable,
   ICollection, IComparable, ISliceable, IRangeItem, ITextFinding
{
   private const string REGEX_DOLLAR_INDEX = "-(< '\\') '$' /(/d+)";

   private string getString()
   {
      var text = lazyString.AsString;

      if (text.Matches(REGEX_DOLLAR_INDEX) is (true, var result))
      {
         Slicer slicer = text;
         string[] stringArray = [.. array.List.Select(i => i.AsString)];

         foreach (var match in result)
         {
            var index = match.FirstGroup.Value().Int32(-1);
            if (index.Between(0).Until(stringArray.Length))
            {
               slicer[match.Index, match.Length] = stringArray[index];
            }
         }

         return slicer.ToString();
      }
      else
      {
         return text;
      }
   }

   private KString getKString() => (KString)getString();

   public string ClassName => "Formatter";

   public string AsString => getString();

   public string Image => getString();

   public int Hash => lazyString.Hash;

   public bool IsEqualTo(IObject obj) => obj.AsString == getString();

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(getKString(), comparisand, bindings);

   public bool IsTrue => getString().IsNotEmpty();

   public Guid Id { get; init; } = Guid.NewGuid();

   public int CompareTo(KString other) => getString().CompareTo(other.Value);

   public bool Equals(KString other) => getString() == other.Value;

   public KString Format(string format) => getKString().Format(format);

   public KString Format(string[] formats) => getKString().Format(formats);

   public IObject this[SkipTake skipTake] => getKString()[skipTake];

   public IIterator GetIterator(bool lazy) => getKString().GetIterator(lazy);

   public Maybe<IObject> Next(int index) => getKString().Next(index);

   public Maybe<IObject> Peek(int index) => getKString().Peek(index);

   Int ICollection.Length => getString().Length;

   public Slice Slice(ICollection collection) => getKString().Slice(collection);

   public Maybe<IObject> Get(IObject index) => getKString().Get(index);

   public IObject Set(IObject index, IObject value) => getKString().Set(index, value);

   public bool ExpandForArray => getKString().ExpandForArray;

   public int Length => getString().Length;

   public KBoolean In(IObject item) => getKString().In(item);

   public KBoolean NotIn(IObject item) => getKString().NotIn(item);

   public IObject Times(int count) => getKString().Times(count);

   public KString MakeString(string connector) => getKString().MakeString(connector);

   public IIterator GetIndexedIterator() => getKString().GetIndexedIterator();

   public IObject One() => getKString().One();

   public IObject Copy() => getKString().Copy();

   public IIterator Following(IObject following) => new MultiIterator(this, following);

   public Maybe<TypeConstraint> TypeConstraint => nil;

   public int CompareTo(object? obj) => getKString().CompareTo(obj);

   public int Compare(IObject obj) => getKString().CompareTo(obj);

   public IObject Object => getKString().Object;

   public KBoolean Between(IObject min, IObject max, bool inclusive) => getKString().Between(min, max, inclusive);

   public KBoolean After(IObject min, IObject max, bool inclusive) => getKString().After(min, max, inclusive);

   public IRangeItem Successor => getKString().Successor;

   public IRangeItem Predecessor => getKString().Predecessor;

   public KRange Range() => getKString().Range();

   public IObject Find(string input, int startIndex, bool reverse) => getKString().Find(input, startIndex, reverse);

   public KArray FindAll(string input) => getKString().FindAll(input);

   public KString Replace(string input, string replacement, bool reverse) => getKString().Replace(input, replacement, reverse);

   public KString Replace(string input, Lambda lambda, bool reverse) => getKString().Replace(input, lambda, reverse);

   public KString ReplaceAll(string input, string replacement) => getKString().ReplaceAll(input, replacement);

   public KString ReplaceAll(string input, Lambda lambda) => getKString().ReplaceAll(input, lambda);

   public KArray Split(string input) => getKString().Split(input);

   public KTuple Partition(string input, bool reverse) => getKString().Partition(input, reverse);

   public Int Count(string input) => getKString().Count(input);

   public Int Count(string input, Lambda lambda) => getKString().Count(input, lambda);

   public Formatter Clone(KArray newArray) => new(lazyString, newArray);
}