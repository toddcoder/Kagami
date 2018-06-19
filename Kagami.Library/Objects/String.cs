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
      IRangeItem
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

      public Boolean Between(IObject min, IObject max) => between(this, min, max);

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

      public String Capital() => value.ToUpper1();

      public Boolean IsPrefix(string substring) => value.StartsWith(substring);

      public Boolean IsSuffix(string substring) => value.EndsWith(substring);

/*      public Boolean In(string substring) => value.Contains(substring);

      public Boolean NotIn(string substring) => !value.Contains(substring);*/

      public String Replace(string old, string @new) => value.Replace(old, @new);

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

      public IObject Position(string substring, int startIndex, bool reverse)
      {
         var index = reverse ? value.LastIndexOf(substring, startIndex, StringComparison.Ordinal)
            : value.IndexOf(substring, startIndex, StringComparison.Ordinal);

         if (index == -1)
            return Nil.NilValue;
         else
            return Some.Object((Int)index);
      }

      public IObject Positions(string substring)
      {
         var list = new List<IObject>();
         var index = 0;
         while (index > -1)
         {
            index = value.IndexOf(substring, index, StringComparison.Ordinal);
            if (index > -1)
            {
               list.Add((Int)index);
               index++;
            }
         }

         return new Tuple(list.ToArray());
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
   }
}