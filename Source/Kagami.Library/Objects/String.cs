using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Kagami.Library.Objects
{
   public readonly struct String : IObject, IComparable<String>, IEquatable<String>, IFormattable, ICollection, IComparable,
      ISliceable,
      IRangeItem, ITextFinding
   {
      public static implicit operator String(string value) => new(value);

      public static IObject StringObject(string value) => new String(value);

      public static IObject Empty => StringObject("");

      private readonly string value;

      public String(string value) : this() => this.value = value;

      public string Value => value;

      public string ClassName => "String";

      public string AsString => value;

      public string Image => $"\"{value.Replace("\"", @"\""").Replace("\t", @"\t")}\"";

      public int Hash => value.GetHashCode();

      public bool IsEqualTo(IObject obj)
      {
         return obj is String s && value == s.value || obj is Regex regex && regex.IsMatch(value).IsTrue;
      }

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => value.Length > 0;

      public int Compare(IObject obj) => CompareTo((String)obj);

      public IObject Object => this;

      public Boolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

      public Boolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

      public int CompareTo(String other) => string.Compare(value, other.value, StringComparison.Ordinal);

      public bool Equals(String other) => string.Equals(value, other.value);

      public override bool Equals(object obj) => obj is String s && Equals(s);

      public override int GetHashCode() => Hash;

      public int CompareTo(object obj) => CompareTo((String)obj);

      public String Format(string format)
      {
         if (format.StartsWith("s."))
         {
            var amount = format.Drop(2);
            if (amount.StartsWith("-"))
            {
               amount = amount.Drop(1);
            }

            return value.Center(ConversionFunctions.Value.Int32(amount));
         }
         else
         {
            return value.FormatAs(format);
         }
      }

      public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

      public Maybe<IObject> Next(int index)
      {
         var self = this;
         return maybe<IObject>() & index < value.Length & (() => Char.CharObject(self.value[index]));
      }

      public Maybe<IObject> Peek(int index)
      {
         var self = this;
         return maybe<IObject>() & index < value.Length & (() => Char.CharObject(self.value[index]));
      }

      public Int Length => value.Length;

      public IEnumerable<IObject> List
      {
         get
         {
            foreach (var c in value)
            {
               yield return (Char)c;
            }
         }
      }

      public Slice Slice(ICollection collection) => new(this, collection.GetIterator(false).List().ToArray());

      public Maybe<IObject> Get(IObject index)
      {
         var intIndex = wrapIndex(((Int)index).Value, value.Length);
         return Next(intIndex);
      }

      public IObject Set(IObject index, IObject value) => throw immutableValue(ClassName);

      public Char this[int index]
      {
         get
         {
            index = wrapIndex(index, value.Length);
            return value[index];
         }
      }

      public String this[Container container]
      {
         get
         {
            var builder = new StringBuilder();
            foreach (var index in indexList(container, value.Length))
            {
               builder.Append(value[index]);
            }

            return builder.ToString();
         }
      }

      public bool ExpandForArray => false;

      int ISliceable.Length => value.Length;

      public Boolean In(IObject item) => item switch
      {
         String other => value.Contains(other.value),
         Char c => value.IndexOf(c.Value) >= 0,
         _ => false
      };

      public Boolean NotIn(IObject item) => item switch
      {
         String other => !value.Contains(other.value),
         Char c => value.IndexOf(c.Value) == -1,
         _ => false
      };

      public IObject Times(int count) => (String)value.Repeat(count);

      public String MakeString(string connector) => makeString(this, connector);

      public IIterator GetIndexedIterator() => new IndexedIterator(this);

      public String Repeat(int count) => value.Repeat(count);

      public Char GetChar(int index) => value[index];

      public String Upper() => value.ToUpper();

      public String Lower() => value.ToLower();

      public String Title() => value.ToTitleCase();

      public String Upper1() => value.ToUpper1();

      public String Lower1() => value.ToLower1();

      public Boolean IsPrefix(string substring) => value.StartsWith(substring);

      public Boolean IsSuffix(string substring) => value.EndsWith(substring);

      public String Replace(string old, string @new)
      {
         var _index = value.Find(old);
         if (_index)
         {
            return value.Keep(_index) + @new + value.Drop(_index.Value + old.Length);
         }
         else
         {
            return new String(value);
         }
      }

      public Tuple FindAll(ITextFinding textFinding) => textFinding.FindAll(value);

      public Tuple FindAll(string input) => findAll(value, input);

      public String Replace(ITextFinding textFinding, string replacement, bool reverse)
      {
         return textFinding.Replace(value, replacement, reverse);
      }

      public String Replace(string input, string replacement, bool reverse) => replace(value, input, replacement, reverse);

      public String Replace(ITextFinding textFinding, Lambda lambda, bool reverse) => textFinding.Replace(value, lambda, reverse);

      public String Replace(string input, Lambda lambda, bool reverse) => replace(value, input, lambda, reverse);

      public String ReplaceAll(ITextFinding textFinding, string replacement) => textFinding.ReplaceAll(value, replacement);

      public String ReplaceAll(string input, string replacement) => replaceAll(value, input, replacement);

      public String ReplaceAll(ITextFinding textFinding, Lambda lambda) => textFinding.ReplaceAll(value, lambda);

      public String ReplaceAll(string input, Lambda lambda) => replaceAll(value, input, lambda);

      public Tuple Split(ITextFinding textFinding) => textFinding.Split(value);

      public Tuple Split(string input) => split(value, input);

      public Tuple Partition(ITextFinding textFinding, bool reverse) => textFinding.Partition(value, reverse);

      public Tuple Partition(string input, bool reverse) => partition(value, input, reverse);

      public Int Count(ITextFinding textFinding) => textFinding.Count(value);

      public Int Count(string input) => count(value, input);

      public Int Count(ITextFinding textFinding, Lambda lambda) => textFinding.Count(value, lambda);

      public Int Count(string input, Lambda lambda) => count(value, input, lambda);

      public String LStrip() => value.TrimStart();

      public String RStrip() => value.TrimEnd();

      public String Strip() => value.Trim();

      public String Center(int width, char padding) => value.Center(width, padding);

      public String Center(int width) => value.Center(width);

      public String LJust(int width, char padding) => value.LeftJustify(width, padding);

      public String LJust(int width) => value.LeftJustify(width);

      public String RJust(int width, char padding) => value.RightJustify(width, padding);

      public String RJust(int width) => value.RightJustify(width);

      public Boolean IsEmpty => value.IsEmpty();

      public Boolean IsNotEmpty => value.IsNotEmpty();

      public String Concatenate(string otherString) => value + otherString;

      public Boolean IsAlphaDigit => value.All(char.IsLetterOrDigit);

      public Boolean IsAlpha => value.All(char.IsLetter);

      public Boolean IsDigit => value.All(char.IsDigit);

      public Boolean IsLower => value.All(char.IsLower);

      public Boolean IsUpper => value.All(char.IsUpper);

      public Boolean IsSpace => value.All(char.IsWhiteSpace);

      public Boolean IsTitle => value == value.ToTitleCase();

      private static string expand(string value)
      {
         var _result = value.Matches("/(.) /'-' /(.)");
         if (_result)
         {
            foreach (var match in _result.Value)
            {
               var left = match.FirstGroup[0];
               var right = match.ThirdGroup[0];
               var builder = new StringBuilder();
               if (left < right)
               {
                  for (var i = left + 1; i < right; i++)
                  {
                     builder.Append((char)i);
                  }
               }
               else
               {
                  for (var i = right - 1; i > left; i--)
                  {
                     builder.Append((char)i);
                  }
               }
            }

            return _result.Value.ToString();
         }
         else
         {
            return value;
         }
      }

      public String Translate(string from, string to)
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
         var table = new AutoHash<char, char>(k => k);

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

      public String Truncate(int width, bool ellipses = true) => value.Truncate(width, ellipses);

      public IObject Find(ITextFinding textFinding, int startIndex, bool reverse) => textFinding.Find(value, startIndex, reverse);

      public IObject Find(string input, int startIndex, bool reverse) => find(value, input, startIndex, reverse);

      public IObject Int() => int.TryParse(value, out var result) ? new Some((Int)result) : None.NoneValue;

      public IObject Float() => double.TryParse(value, out var result) ? new Some((Float)result) : None.NoneValue;

      public IObject Byte() => byte.TryParse(value, out var result) ? new Some((Byte)result) : None.NoneValue;

      public IObject Long() => BigInteger.TryParse(value, out var result) ? new Some((Long)result) : None.NoneValue;

      public Tuple SplitRegex(Regex regex) => regex.Split(value);

      public Tuple SplitOn(string on) => new(value.Split(new[] { on }, StringSplitOptions.None).Select(StringObject).ToArray());

      public String Subtract(string substring) => value.Replace(substring, "");

      public IRangeItem Successor => (String)value.Succ();

      public IRangeItem Predecessor => (String)value.Pred();

      public Range Range() => new(this, (String)"z".Repeat(value.Length), true);

      public String Get() => value.get();

      public String Set() => value.set();

      public String SwapCase()
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

      public Tuple Fields => new(value.Unjoin("/s+").Select(StringObject).ToArray());

      public IObject Words(int count)
      {
         var _result = value.Matches("/w+");
         if (_result)
         {
            var index = -1;
            var length = 0;
            var list = new List<string>();
            for (var i = 0; i < _result.Value.MatchCount.MinOf(count); i++)
            {
               list.Add(_result.Value[i]);
               var (_, index1, length1) = _result.Value.GetMatch(i);
               index = index1;
               length = length1;
            }

            var remainder = value.Drop(index + length);
            list.Add(remainder.TrimStart());

            return Some.Object(new Tuple(list.Select(StringObject).ToArray()));
         }
         else
         {
            return None.NoneValue;
         }
      }

      public Tuple Words() => new(value.Unjoin("/s+").Select(StringObject).ToArray());

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
   }
}