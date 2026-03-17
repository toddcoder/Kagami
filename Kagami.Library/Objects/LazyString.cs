using Core.Collections;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct LazyString(string value) : IObject, IComparable<KString>, IEquatable<KString>, IFormattable,
   ICollection, IComparable, ISliceable, IRangeItem, ITextFinding
{
   private string getString()
   {
      if (value.Matches("-(<'\\') /('$') /(['A-Za-z_']['A-Za-z_0-9']*) ('[' /(-[']']+) ']')?") is (true, var results))
      {
         Slicer slicer = value;
         foreach (var match in results)
         {
            var fieldName = match.SecondGroup;
            var format = match.ThirdGroup;

            var _field = Machine.Current.Find(fieldName, true);
            if (_field is (true, var field))
            {
               var fieldValue = field.Value;
               string fieldString;
               if (format.IsNotEmpty() && fieldValue is IFormattable formattable)
               {
                  fieldString = formattable.Format(format).AsString;
               }
               else
               {
                  fieldString = fieldValue.AsString;
               }

               var group1 = match.Groups[1];
               var group2 = match.Groups[2];
               var group3 = match.Groups[3];
               slicer[group1.Index, group1.Length] = "";
               slicer[group2.Index, group2.Length] = fieldString;
               slicer[group3.Index, group3.Length] = "";
            }
         }

         return slicer.ToString().Replace(@"\$", "$");
      }
      else
      {
         return value.Replace(@"\$", "$");
      }
   }

   private KString getKString() => (KString)getString();

   public string ClassName => "LazyString";

   public string AsString => getString();

   public string Image => getString();

   public int Hash => value.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj.AsString == getString();

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(getKString(), comparisand, bindings);

   public bool IsTrue => getString().IsNotEmpty();

   public Guid Id { get; init; } = Guid.NewGuid();

   public int CompareTo(KString other) => getString().CompareTo(other.Value);

   public bool Equals(KString other) => getString() == other.Value;

   public KString Format(string format) => getKString().Format(format);

   public KString Format(string[] formats) => format(this, formats);

   public KString Format(Lambda lambda) => format(this, lambda);

   public IObject this[SkipTake skipTake] => getKString()[skipTake];

   public IIterator GetIterator(bool lazy) => getKString().GetIterator(lazy);

   public Maybe<IObject> Next(int index) => getKString().Next(index);

   public Maybe<IObject> Peek(int index) => getKString().Peek(index);

   Int ICollection.Length => getString().Length;

   public Slice Slice(ICollection collection) => getKString().Slice(collection);

   public Maybe<IObject> Get(IObject index) => getKString().Get(index);

   public IObject Set(IObject index, IObject svalue) => getKString().Set(index, svalue);

   public bool ExpandForArray => getKString().ExpandForArray;

   public int Length => getString().Length;

   public KBoolean In(IObject item) => getKString().In(item);

   public KBoolean NotIn(IObject item) => getKString().NotIn(item);

   public IObject Times(int count) => getKString().Times(count);

   public KString MakeString(string connector) => getKString().MakeString(connector);

   public IIterator GetIndexedIterator() => getKString().GetIndexedIterator();

   public IObject One() => getKString();

   public IObject Copy() => getKString().Copy();

   public IIterator Following(IObject following) => new MultiIterator(this, following);

   public Maybe<TypeConstraint> TypeConstraint => nil;

   public int CompareTo(object? obj) => getKString().CompareTo(obj);

   public int Compare(IObject obj) => getKString().Compare(obj);

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
}