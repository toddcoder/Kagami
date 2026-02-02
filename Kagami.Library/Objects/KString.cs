using System.Globalization;
using System.Numerics;
using System.Text;
using Core.Collections;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Objects.TextFindingFunctions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;
using Core.Matching;
using Core.Numbers;

namespace Kagami.Library.Objects;

public readonly struct KString : IObject, IComparable<KString>, IEquatable<KString>, IFormattable, ICollection, IComparable, ISliceable, IRangeItem,
   ITextFinding, IIndexed, IAccepting
{
   public static implicit operator KString(string value) => new(value);

   public static IObject StringObject(string value) => new KString(value);

   public static IObject Empty => StringObject("");

   private readonly string value;

   public KString(string value) : this()
   {
      this.value = value;
   }

   public string Value => value;

   public string ClassName => "String";

   public string AsString => value;

   public string Image => $"\"{value.Replace("\"", @"\""").Replace("\t", @"\t").Replace("\n", @"\n").Replace("\r", @"\r")}\"";

   public int Hash => value.GetHashCode();

   public bool IsEqualTo(IObject obj)
   {
      return obj is KString s && value == s.value || obj is Regex regex && regex.IsMatch(value).IsTrue;
   }

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => value.Length > 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public int Compare(IObject obj) => CompareTo((KString)obj);

   public IObject Object => this;

   public KBoolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

   public KBoolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

   public int CompareTo(KString other) => string.Compare(value, other.value, StringComparison.Ordinal);

   public bool Equals(KString other) => string.Equals(value, other.value);

   public override bool Equals(object? obj) => obj is KString s && Equals(s);

   public override int GetHashCode() => Hash;

   public IObject Accept(IObject obj) => obj switch
   {
      Int i => KBoolean.BooleanObject(value.Length == i.Value),
      KChar c => KBoolean.BooleanObject(value.Contains(c.Value)),
      KString s => KBoolean.BooleanObject(value.Contains(s.value)),
      Regex r => r.Matches(value),
      _ => throw cannotAccept(obj)
   };

   public int CompareTo(object? obj) => CompareTo((KString)obj!);

   public KString Format(string format)
   {
      if (format.StartsWith("s."))
      {
         var amount = format.Drop(2);
         if (amount.StartsWith("-"))
         {
            amount = amount.Drop(1);
         }

         return value.Center(amount.Value().Int32());
      }
      else
      {
         return (KString)value.FormatUsing(format, obj => obj);
      }
   }

   public KString Format(string[] formats) => format(this, formats);

   public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

   public Maybe<IObject> Next(int index)
   {
      var self = this;
      return maybe<IObject>() & index < value.Length & (() => KChar.CharObject(self.value[index]));
   }

   public Maybe<IObject> Peek(int index)
   {
      var self = this;
      return maybe<IObject>() & index < value.Length & (() => KChar.CharObject(self.value[index]));
   }

   IObject IIndexed.this[int index]
   {
      get => this[index];
      set => throw fail("String is immutable");
   }

   public IObject this[KIndex index]
   {
      get => index.GetFromCollection(this);
      set => throw fail("String is immutable");
   }

   public int LastIndex => value.Length - 1;

   int IIndexed.Length => value.Length;

   public KIndex Start => KIndex.StartIndex(this);

   public KIndex End => KIndex.EndIndex(this);

   public KIndex Indexes => KIndex.FullIndex(this);

   public Int Length => value.Length;

   public IEnumerable<IObject> List => value.Select(c => (KChar)c).Cast<IObject>();

   public Slice Slice(ICollection collection) => new(this, collection.GetIterator(false).List().ToArray());

   public Maybe<IObject> Get(IObject index)
   {
      var intIndex = wrapIndex(((Int)index).Value, value.Length);
      return Next(intIndex);
   }

   public IObject Set(IObject index, IObject value) => throw immutableValue(ClassName);

   public KChar this[int index]
   {
      get
      {
         index = wrapIndex(index, value.Length);
         return value[index];
      }
   }

   public KString this[Sequence sequence]
   {
      get
      {
         var builder = new StringBuilder();
         foreach (var index in indexList(sequence, value.Length))
         {
            builder.Append(value[index]);
         }

         return builder.ToString();
      }
   }

   public bool ExpandForArray => false;

   int ISliceable.Length => value.Length;

   public KBoolean In(IObject item) => item switch
   {
      KString other => value.Contains(other.value),
      KChar c => value.IndexOf(c.Value) >= 0,
      _ => false
   };

   public KBoolean NotIn(IObject item) => item switch
   {
      KString other => !value.Contains(other.value),
      KChar c => value.IndexOf(c.Value) == -1,
      _ => false
   };

   public IObject Times(int count) => (KString)value.Repeat(count);

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IObject One() => this;

   public IObject Copy() => new KString(value);

   public IIterator Following(IObject following) => new MultiIterator(this, following);

   public Maybe<TypeConstraint> TypeConstraint => nil;

   public KString Repeat(int count) => value.Repeat(count);

   public KChar GetChar(int index) => value[index];

   public KString Upper() => value.ToUpper();

   public KString Lower() => value.ToLower();

   public KString Title() => value.ToTitleCase();

   public KString Upper1() => value.ToUpper1();

   public KString Lower1() => value.ToLower1();

   public KString Camel() => value.ToCamel();

   public KString Pascal() => value.ToPascal();

   public KBoolean IsPrefix(string substring) => value.StartsWith(substring);

   public KBoolean IsSuffix(string substring) => value.EndsWith(substring);

   public KString Replace(KString old, KString @new, bool ignoreCase) => value.Replace(old.value, @new.value, ignoreCase, CultureInfo.CurrentCulture);

   public KArray FindAll(ITextFinding textFinding) => textFinding.FindAll(value);

   public KArray FindAll(string input) => findAll(value, input);

   public KString Replace(ITextFinding textFinding, string replacement, bool reverse)
   {
      return textFinding.Replace(value, replacement, reverse);
   }

   public KString Replace(string input, string replacement, bool reverse) => replace(value, input, replacement, reverse);

   public KString Replace(ITextFinding textFinding, Lambda lambda, bool reverse) => textFinding.Replace(value, lambda, reverse);

   public KString Replace(string input, Lambda lambda, bool reverse) => replace(value, input, lambda, reverse);

   public KString ReplaceAll(ITextFinding textFinding, string replacement) => textFinding.ReplaceAll(value, replacement);

   public KString ReplaceAll(string input, string replacement) => replaceAll(value, input, replacement);

   public KString ReplaceAll(ITextFinding textFinding, Lambda lambda) => textFinding.ReplaceAll(value, lambda);

   public KString ReplaceAll(string input, Lambda lambda) => replaceAll(value, input, lambda);

   public KString ReplaceAll(Dictionary dictionary)
   {
      var buffer = new StringBuilder(value);
      foreach (var (find, replace) in dictionary.InternalHash)
      {
         buffer.Replace(find.AsString, replace.AsString);
      }

      return (KString)buffer.ToString();
   }

   public KArray Split(ITextFinding textFinding) => textFinding.Split(value);

   public KArray Split(string input) => split(value, input);

   public KTuple Partition(ITextFinding textFinding, bool reverse) => textFinding.Partition(value, reverse);

   public KTuple Partition(string input, bool reverse) => partition(value, input, reverse);

   public Int Count(ITextFinding textFinding) => textFinding.Count(value);

   public Int Count(string input) => count(value, input);

   public Int Count(ITextFinding textFinding, Lambda lambda) => textFinding.Count(value, lambda);

   public Int Count(string input, Lambda lambda) => count(value, input, lambda);

   public KString LStrip() => value.TrimStart();

   public KString RStrip() => value.TrimEnd();

   public KString Strip() => value.Trim();

   public KString Center(int width, char padding) => value.Center(width, padding);

   public KString Center(int width) => value.Center(width);

   public KString LJust(int width, char padding) => value.LeftJustify(width, padding);

   public KString LJust(int width) => value.LeftJustify(width);

   public KString RJust(int width, char padding) => value.RightJustify(width, padding);

   public KString RJust(int width) => value.RightJustify(width);

   public KBoolean IsEmpty => value.IsEmpty();

   public KBoolean IsNotEmpty => value.IsNotEmpty();

   public KString Concatenate(string otherString) => value + otherString;

   public KBoolean IsAlphaDigit => value.All(char.IsLetterOrDigit);

   public KBoolean IsAlpha => value.All(char.IsLetter);

   public KBoolean IsDigit => value.All(char.IsDigit);

   public KBoolean IsLower => value.All(char.IsLower);

   public KBoolean IsUpper => value.All(char.IsUpper);

   public KBoolean IsSpace => value.All(char.IsWhiteSpace);

   public KBoolean IsTitle => value == value.ToTitleCase();

   private static string expand(string value)
   {
      var _result = value.Matches("/(.) /'-' /(.)");
      if (_result is (true, var result))
      {
         foreach (var match in result)
         {
            var left = match.FirstGroup[0];
            var right = match.ThirdGroup[0];
            var builder = new StringBuilder();
            if (left < right)
            {
               builder.Append(left);

               for (var i = left + 1; i < right; i++)
               {
                  builder.Append((char)i);
               }

               builder.Append(right);
            }
            else
            {
               builder.Append(right);

               for (var i = right - 1; i > left; i--)
               {
                  builder.Append((char)i);
               }

               builder.Append(left);
            }

            return builder.ToString();
         }

         return result.Text;
      }
      else
      {
         return value;
      }
   }

   public KString Translate(string from, string to)
   {
      from = expand(from);
      to = expand(to);
      var fromLength = from.Length;
      var toLength = to.Length;
      if (toLength < fromLength)
      {
         var count = fromLength - toLength;
         var remainder = to[toLength - 1].Repeat(count);
         to += remainder;
         toLength = to.Length;
      }

      var length = Math.Min(fromLength, toLength);
      var table = new Memo<char, char>.Function(k => k);

      for (var i = 0; i < length; i++)
      {
         table[from[i]] = to[i];
      }

      var result = new StringBuilder();
      foreach (var ch in value)
      {
         result.Append(table[ch]);
      }

      return result.ToString();
   }

   public KString Translate(Dictionary dictionary, bool omit)
   {
      var builder = new StringBuilder();
      foreach (var ch in value)
      {
         var result = dictionary[(KChar)ch];
         if (result is Some some)
         {
            builder.Append(((KChar)some.Value).Value);
         }
         else if (!omit)
         {
            builder.Append(ch);
         }
      }

      return builder.ToString();
   }

   public KString Truncate(int width, bool ellipses = true) => value.Truncate(width, ellipses);

   public IObject Find(ITextFinding textFinding, int startIndex, bool reverse) => textFinding.Find(value, startIndex, reverse);

   public IObject Find(string input, int startIndex, bool reverse) => find(value, input, startIndex, reverse);

   public IObject Int() =>
      int.TryParse(value, out var result) ? new Some((Int)result, Objects.TypeConstraint.SingleType(nameof(Int))) : KNil.NilValue;

   public IObject Float() => double.TryParse(value, out var result) ? new Some((Float)result, Objects.TypeConstraint.SingleType(nameof(Float)))
      : KNil.NilValue;

   public IObject Byte() => byte.TryParse(value, out var result) ? new Some((KByte)result, Objects.TypeConstraint.SingleType("Byte")) : KNil.NilValue;

   public IObject Long() => BigInteger.TryParse(value, out var result) ? new Some((Long)result, Objects.TypeConstraint.SingleType(nameof(Long)))
      : KNil.NilValue;

   public KArray SplitRegex(Regex regex) => regex.Split(value);

   public KArray SplitOn(string on) => new(value.Split([on], StringSplitOptions.None).Select(StringObject));

   public KArray SplitOn(ICollection collection, bool keepRest)
   {
      List<IObject> result = [];
      var iterator = collection.GetIterator(false);
      var input = value;
      while (iterator.Next() is (true, Int i))
      {
         result.Add(StringObject(input.Keep(i.Value)));
         input = input.Drop(i.Value);
      }

      if (input.IsNotEmpty() && keepRest)
      {
         result.Add(StringObject(input));
      }

      return new KArray(result);
   }

   public KString Subtract(string substring) => value.Replace(substring, "");

   public KString Subtract(KRange range)
   {
      StringBuilder builder = new(value);
      foreach (var obj in range.GetIterator(false).List())
      {
         builder.Replace(obj.AsString, "");
      }

      return builder.ToString();
   }

   public IRangeItem Successor => (KString)value.Succ();

   public IRangeItem Predecessor => (KString)value.Pred();

   public KRange Range() => new(this, (KString)"z".Repeat(value.Length), true);

   public KString Get() => value.get();

   public KString Set() => value.set();

   public KString SwapCase()
   {
      var builder = new StringBuilder();
      foreach (var ch in value)
      {
         if (char.IsLetter(ch))
         {
            builder.Append(char.IsLower(ch) ? char.ToUpper(ch) : char.ToLower(ch));
         }
         else
         {
            builder.Append(ch);
         }
      }

      return builder.ToString();
   }

   public IObject Words(int count)
   {
      var _result = value.Matches("/w+");
      if (_result is (true, var result))
      {
         var index = -1;
         var length = 0;
         List<string> list = [];
         for (var i = 0; i < result.MatchCount.MinOf(count); i++)
         {
            list.Add(result[i]);
            var (_, index1, length1) = result.GetMatch(i);
            index = index1;
            length = length1;
         }

         var remainder = value.Drop(index + length);
         list.Add(remainder.TrimStart());

         return Some.Object(new KTuple(list.Select(StringObject).ToArray()), Objects.TypeConstraint.SingleType("Tuple"));
      }
      else
      {
         return KNil.NilValue;
      }
   }

   public IObject Words() => new Words(this);

   public IObject Word(int index) => ((Words)Words()).GetIterator(false).ToArray()[index];

   public MutString Append(IObject obj)
   {
      var mutString = new MutString(value);
      mutString.Append(obj);

      return mutString;
   }

   public MutString Mutable() => new(value);

   public IObject this[SkipTake skipTake]
   {
      get
      {
         var (skip, take) = skipTake;
         return StringObject(value.Drop(skip).Keep(take));
      }
   }

   public KString Succ()
   {
      var _index = value.Find(".");
      if (_index is (true, var index))
      {
         var left = value.Keep(index).Succ();
         return left + value.Drop(index);
      }
      else
      {
         return value.Succ();
      }
   }

   public KString Pred()
   {
      var _index = value.Find(".");
      if (_index is (true, var index))
      {
         var left = value.Keep(index).Pred();
         return left + value.Drop(index);
      }
      else
      {
         return value.Pred();
      }
   }

   public KString Squeeze()
   {
      var set = new Set<char>();
      foreach (var ch in value)
      {
         set.Add(ch);
      }

      return new KString(new string([.. set]));
   }

   public KString PadLeft(int length, char paddingChar = ' ') => value.PadLeft(length, paddingChar);

   public KString PadRight(int length, char paddingChar = ' ') => value.PadRight(length, paddingChar);

   public KString PadCenter(int length, char paddingChar = ' ') => value.PadCenter(length, paddingChar);

   public KString Head => (KString)value.Keep(1);

   public KString Tail => (KString)value.Drop(1);

   public KString Margin() => value.Substitute("[' ' /t]* '|'", "");

   public IObject ParseBase(int @base)
   {
      try
      {
         return Success.Object((Int)Convert.ToInt32(value, @base), Objects.TypeConstraint.SingleType(nameof(Objects.Int)));
      }
      catch (Exception exception)
      {
         return new Failure(exception.Message);
      }
   }

   public KString WordCase()
   {
      var _result = value.Matches("/w+");
      if (_result is (true, var result))
      {
         foreach (var match in result)
         {
            match.Text = match.Text.ToUpper1();
         }

         return result.Text;
      }
      else
      {
         return this;
      }
   }

   public ByteArray Encode(string encodingName)
   {
      var encoding = Encoding.GetEncoding(encodingName);
      return new ByteArray(encoding.GetBytes(value));
   }

   public PendingRegex PendingRegex(Regex regex) => new(regex, this);

   public IObject Fields() => new FieldsIterator(this);

   public IObject Fields(Regex regex) => new FieldsIterator(this, regex);

   public IObject Field(int index) => ((IIterator)Fields()).ToArray()[index];

   public IObject Numberize()
   {
      if (value.IsMatch("['.e']"))
      {
         return Objects.Float.FloatObject(value.Value().Double());
      }
      else if (value.IsDate())
      {
         return Date.DateObject(value.Value().DateTime());
      }
      else
      {
         return Objects.Int.IntObject(value.Value().Int32());
      }
   }

   public IObject Lines() => new LinesIterator(this);

   public IObject Assign(SkipTake skipTake, KString source)
   {
      var array = value.ToCharArray();
      char[] left = [.. array.Take(skipTake.Skip)];
      char[] middle = [.. source.GetIterator(false).List().Select(c => (KChar)c).Select(kc => kc.Value)];
      char[] right = [.. array.Skip(skipTake.Skip + skipTake.Take)];

      var result = new StringBuilder();
      result.Append(left);
      result.Append(middle);
      result.Append(right);

      return StringObject(result.ToString());
   }

   public KString ExpandTabs(int size = 8)
   {
      var expanded = " ".Repeat(size);
      return value.Replace("\t", expanded);
   }

   public KString Capitalize() => value.Keep(1).ToUpper() + value.Drop(1).ToLower();

   public KString Insert(string substring, int index)
   {
      var builder = new StringBuilder();
      builder.Append(value.Keep(index));
      builder.Append(substring);
      builder.Append(value.Drop(index));

      return builder.ToString();
   }

   public KString Delete(int index, int length)
   {
      var builder = new StringBuilder();
      var kept = value.Keep(index);
      builder.Append(kept);
      builder.Append(value.Drop(index + length));

      return new KString(builder.ToString());
   }

   public KString Mapping(string from, string to)
   {
      Hash<char, char> hash = [];
      var length = value.Length.MinOf(from.Length);
      for (var i = 0; i < length; i++)
      {
         hash[from[i]] = value[i];
      }

      var builder = new StringBuilder();
      foreach (var ch in to)
      {
         if (hash.Maybe[ch] is (true, var found))
         {
            builder.Append(found);
         }
         else
         {
            builder.Append(ch);
         }
      }

      return builder.ToString();
   }
}