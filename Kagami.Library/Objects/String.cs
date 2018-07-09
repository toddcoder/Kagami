using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using Standard.Types.Objects;
using Standard.Types.Strings;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Objects
{
   public struct String : IObject, IComparable<String>, IEquatable<String>, IFormattable, ICollection, IComparable, ISliceable,
      IRangeItem, ITextFinding
   {
      public static implicit operator String(string value) => new String(value);

      public static IObject StringObject(string value) => new String(value);

      public static IObject Empty => StringObject("");

      string value;

      public String(string value) : this() => this.value = value;

      public string Value => value;

      public string ClassName => "String";

      public string AsString => value;

      public string Image => $"\"{value.Replace("\"", @"\""").Replace("\t", @"\t")}\"";

      public int Hash => value.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is String s && value == s.value;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => value.Length > 0;

      public int Compare(IObject obj) => CompareTo((String)obj);

      public IObject Object => this;

      public Boolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

      public int CompareTo(String other) => value.CompareTo(other.value);

      public bool Equals(String other) => string.Equals(value, other.value);

      public override bool Equals(object obj) => obj is String s && Equals(s);

      public override int GetHashCode() => Hash;

      public int CompareTo(object obj) => CompareTo((String)obj);

      public String Format(string format) => value.FormatAs(format);

      public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

      public IMaybe<IObject> Next(int index)
      {
         var self = this;
         return when(index < value.Length, () => Char.CharObject(self.value[index]));
      }

      public IMaybe<IObject> Peek(int index)
      {
         var self = this;
         return when(index < value.Length, () => Char.CharObject(self.value[index]));
      }

      public Int Length => value.Length;

      public IEnumerable<IObject> List
      {
         get
         {
            foreach (var c in value)
               yield return (Char)c;
         }
      }

      public Slice Slice(ICollection collection) => new Slice(this, collection.GetIterator(false).List().ToArray());

      public IMaybe<IObject> Get(IObject index)
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

      public bool ExpandForArray => false;

      int ISliceable.Length => value.Length;

      public Boolean In(IObject item)
      {
         return item is String other && value.Contains(other.value) || item is Char c && value.IndexOf(c.Value) > 0;
      }

      public Boolean NotIn(IObject item)
      {
         return item is String other && !value.Contains(other.value) || item is Char c && value.IndexOf(c.Value) == -1;
      }

      public IObject Times(int count) => (String)value.Repeat(count);

      public IObject Flatten() => flatten(this);

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
         if (value.Find(old).If(out var index))
            return value.Take(index) + @new + value.Skip(index + old.Length);
         else
            return new String(value);
      }

      public Tuple FindAll(ITextFinding textFinding) => textFinding.FindAll(value);

      public Tuple FindAll(string input) => new Tuple(input.FindAll(value).Select(Objects.Int.IntObject).ToArray());

      public String Replace(ITextFinding textFinding, string replacement, bool reverse)
      {
         return textFinding.Replace(value, replacement, reverse);
      }

      public String Replace(string input, string replacement, bool reverse)
      {
         int index;
         if (reverse)
            index = input.LastIndexOf(value, StringComparison.Ordinal);
         else
            index = input.IndexOf(value, StringComparison.Ordinal);

         if (index > -1)
            return input.Take(index) + replacement + input.Skip(index + value.Length);
         else
            return input;
      }

      public String Replace(ITextFinding textFinding, Lambda lambda, bool reverse) => textFinding.Replace(value, lambda, reverse);

      public String Replace(string input, Lambda lambda, bool reverse)
      {
         int index;
         if (reverse)
            index = input.LastIndexOf(value, StringComparison.Ordinal);
         else
            index = input.IndexOf(value, StringComparison.Ordinal);

         if (index > -1)
         {
            var text = input.Skip(index);
            var length = text.Length;
            var replacement = lambda.Invoke((String)text, (Int)index, (Int)length);

            return input.Take(index) + replacement + input.Skip(index + value.Length);
         }
         else
            return input;
      }

      public String ReplaceAll(ITextFinding textFinding, string replacement) => textFinding.ReplaceAll(value, replacement);

      public String ReplaceAll(string input, string replacement) => input.Replace(value, replacement);

      public String ReplaceAll(ITextFinding textFinding, Lambda lambda) => textFinding.ReplaceAll(value, lambda);

      public String ReplaceAll(string input, Lambda lambda)
      {
         var builder = new StringBuilder();
         var index = input.IndexOf(value);
         var start = 0;
         while (index > -1)
         {
            var replacement = lambda.Invoke((String)value, (Int)index, (Int)value.Length);
            builder.Append(input.Skip(start));
            builder.Append(replacement.AsString);
            start = index + value.Length;
            index = input.IndexOf(value);
         }

         builder.Append(input.Skip(start));

         return builder.ToString();
      }

      public Tuple Split(ITextFinding textFinding) => textFinding.Split(value);

      public Tuple Split(string input)
      {
         return new Tuple(input.Split(new[] { value }, StringSplitOptions.None).Select(StringObject).ToArray());
      }

      public Tuple Partition(ITextFinding textFinding, bool reverse) => textFinding.Partition(value, reverse);

      public Tuple Partition(string input, bool reverse)
      {
         if (reverse)
         {
            var index = input.LastIndexOf(value, StringComparison.Ordinal);
            if (index > -1)
               return Tuple.Tuple3(input.Take(index), value, input.Skip(index + value.Length));
            else
               return Tuple.Tuple3(input, "", "");
         }
         else
         {
            if (input.Find(value).If(out var index))
               return Tuple.Tuple3(input.Take(index), value, input.Skip(index + value.Length));
            else
               return Tuple.Tuple3(input, "", "");
         }
      }

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

      public String Translate(string from, string to)
      {
         var length = Math.Min(from.Length, to.Length);
         var table = new AutoHash<char, char>(k => k);

         for (var i = 0; i < length; i++)
            table[from[i]] = to[i];

         var result = new StringBuilder();
         foreach (var ch in value)
            result.Append(table[ch]);

         return result.ToString();
      }

      public String Truncate(int width, bool ellipses = true) => value.Truncate(width, ellipses);

      public IObject Find(ITextFinding textFinding, int startIndex, bool reverse) => textFinding.Find(value, startIndex, reverse);

      public IObject Find(string input, int startIndex, bool reverse)
      {
         int index;
         if (reverse)
         {
            if (startIndex == 0)
               index = input.LastIndexOf(value, StringComparison.Ordinal);
            else
               index = input.LastIndexOf(value, startIndex, StringComparison.Ordinal);
         }
         else
            index = input.IndexOf(value, startIndex, StringComparison.Ordinal);

         if (index == -1)
            return Nil.NilValue;
         else
            return Some.Object((Int)index);
      }

      public IObject Int()
      {
         if (int.TryParse(value, out var result))
            return new Some((Int)result);
         else
            return Nil.NilValue;
      }

      public IObject Float()
      {
         if (double.TryParse(value, out var result))
            return new Some((Float)result);
         else
            return Nil.NilValue;
      }

      public IObject Byte()
      {
         if (byte.TryParse(value, out var result))
            return new Some((Byte)result);
         else
            return Nil.NilValue;
      }

      public IObject Long()
      {
         if (BigInteger.TryParse(value, out var result))
            return new Some((Long)result);
         else
            return Nil.NilValue;
      }

      public Tuple SplitRegex(Regex regex) => regex.Split(value);

      public Tuple SplitOn(string on)
      {
         return new Tuple(value.Split(new[] { on }, StringSplitOptions.None).Select(StringObject).ToArray());
      }

      public String Subtract(string substring) => value.Replace(substring, "");

      public IRangeItem Successor => (String)value.Succ();

      public IRangeItem Predecessor => (String)value.Pred();

      public Range Range() => new Range(this, (String)"z".Repeat(value.Length), true);

      public String Get() => value.get();

      public String Set() => value.set();

      public String SwapCase()
      {
         var builder = new StringBuilder();
         foreach (var ch in value)
            if (char.IsLetter(ch))
               if (char.IsLower(ch))
                  builder.Append(char.ToUpper(ch));
               else
                  builder.Append(char.ToLower(ch));
            else
               builder.Append(ch);

         return builder.ToString();
      }
   }
}