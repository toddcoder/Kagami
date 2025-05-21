using Kagami.Library.Objects;
using Core.Arrays;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Kagami.Awk.AwkFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Objects.KString;
using static Kagami.Library.Objects.TextFindingFunctions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;
using CollectionFunctions = Kagami.Library.Objects.CollectionFunctions;

namespace Kagami.Awk;

public class AwkRecord : IObject, ICollection, ITextFinding
{
   protected string[] fields;
   protected Regex fieldPattern;
   protected string fieldSeparator;

   public AwkRecord(string[] fields, Regex fieldPattern, string fieldSeparator)
   {
      this.fields = fields;
      this.fieldPattern = fieldPattern;
      this.fieldSeparator = fieldSeparator;
   }

   public KString this[int index]
   {
      get => index.Between(0).Until(fields.Length) ? fields[index] : "";
      set
      {
         if (index == 0)
         {
            fields = awkSplit(value.Value, fieldPattern);
         }
         else if (index.Between(1).Until(fields.Length))
         {
            fields[index] = value.Value;
         }
         else
         {
            fields = awkInsert(fields, index, value.Value);
         }
      }
   }

   public string ClassName => "AwkRecord";

   public string AsString => fields.Skip(1).ToString(fieldSeparator).Elliptical(80, ' ');

   public string Image => $"AwkRecord({fields.ToString(", ")}, {fieldSeparator})";

   public int Hash => HashCode.Combine(fields, fieldSeparator);

   public bool IsEqualTo(IObject obj)
   {
      return obj is AwkRecord awkRecord && compareEnumerables(fields, awkRecord.fields) &&
         fieldSeparator == awkRecord.fieldSeparator;
   }

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => fields.Length > 0;

   public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

   public Maybe<IObject> Next(int index) => maybe<IObject>() & index < fields.Length & (() => StringObject(fields[index]));

   public Maybe<IObject> Peek(int index) => Next(index);

   public Int Length => fields.Length;

   public bool ExpandForArray => false;

   public KBoolean In(IObject item) => fields.ContainsValue(item.AsString);

   public KBoolean NotIn(IObject item) => !In(item).Value;

   public IObject Times(int count) => this;

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IObject Find(string input, int startIndex, bool reverse) => find(fields[0], input, startIndex, reverse);

   public IObject Find(ITextFinding textFinding, int startIndex, bool reverse) => textFinding.Find(fields[0], startIndex, reverse);

   public KTuple FindAll(string input) => findAll(fields[0], input);

   public KTuple FindAll(ITextFinding textFinding, string input) => textFinding.FindAll(input);

   public KString Replace(string input, string replacement, bool reverse) => replace(fields[0], input, replacement, reverse);

   public KString Replace(ITextFinding textFinding, string replacement, bool reverse)
   {
      return textFinding.Replace(fields[0], replacement, reverse);
   }

   public KString Replace(string input, Lambda lambda, bool reverse) => replace(fields[0], input, lambda, reverse);

   public KString Replace(ITextFinding textFinding, Lambda lambda, bool reverse) => textFinding.Replace(fields[0], lambda, reverse);

   public KString ReplaceAll(string input, string replacement) => replaceAll(fields[0], input, replacement);

   public KString ReplaceAll(ITextFinding textFinding, string replacement) => textFinding.ReplaceAll(fields[0], replacement);

   public KString ReplaceAll(string input, Lambda lambda) => replaceAll(fields[0], input, lambda);

   public KString ReplaceAll(ITextFinding textFinding, Lambda lambda) => textFinding.ReplaceAll(fields[0], lambda);

   public KTuple Split(string input) => split(fields[0], input);

   public KTuple Split(ITextFinding textFinding) => textFinding.Split(fields[0]);

   public KTuple Partition(string input, bool reverse) => partition(fields[0], input, reverse);

   public KTuple Partition(ITextFinding textFinding, bool reverse) => textFinding.Partition(fields[0], reverse);

   public Int Count(string input) => count(fields[0], input);

   public Int Count(ITextFinding textFinding) => textFinding.Count(fields[0]);

   public Int Count(string input, Lambda lambda) => count(fields[0], input, lambda);

   public Int Count(ITextFinding textFinding, Lambda lambda) => textFinding.Count(fields[0], lambda);

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);
}