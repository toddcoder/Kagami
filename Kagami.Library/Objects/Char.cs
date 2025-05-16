using System;
using System.Linq;
using System.Text;
using Core.Collections;
using Core.Matching;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Char : IObject, IComparable<Char>, IEquatable<Char>, IRangeItem, ITextFinding
{
   public static implicit operator Char(char value) => new(value);

   public static IObject CharObject(char value) => new Char(value);

   private readonly char value;

   public Char(char value) : this() => this.value = value;

   public char Value => value;

   public string ClassName => "Char";

   public string AsString => value.ToString();

   public string Image => $"'{value}'";

   public int Hash => value.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Char c && value == c.value;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => value > 0;

   public int Compare(IObject obj) => CompareTo((Char)obj);

   public IObject Object => this;

   public Boolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

   public Boolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

   public int CompareTo(Char other) => value.CompareTo(other.value);

   public bool Equals(Char other) => value == other.value;

   public override bool Equals(object obj) => obj is Char c && Equals(c);

   public override int GetHashCode() => Hash;

   public String Repeat(int count) => value.Repeat(count);

   public Char Add(IObject c) => c switch
   {
      Char ch => add(ch.value),
      Int count => add(count.Value),
      _ => throw incompatibleClasses(c, "Char or Int")
   };

   public Char Subtract(IObject c) => c switch
   {
      Char ch => subtract(ch.value),
      Int count => subtract(count.Value),
      _ => throw incompatibleClasses(c, "Char or Int")
   };

   public Char add(int count) => (char)(value + count);

   public Char add(char ch) => (char)(value + ch);

   public Char subtract(int count) => (char)(value - count);

   public Char subtract(char ch) => (char)(value - ch);

   public Char Upper() => char.ToUpper(value);

   public Char Lower() => char.ToLower(value);

   public Boolean IsAlphaDigit => char.IsLetterOrDigit(value);

   public Boolean IsAlpha => char.IsLetter(value);

   public Boolean IsDigit => char.IsDigit(value);

   public Boolean IsLower => char.IsLower(value);

   public Boolean IsUpper => char.IsUpper(value);

   public Boolean IsSpace => char.IsWhiteSpace(value);

   public Boolean IsVowel => value.ToString().IsMatch("['aeiou']; i");

   public Boolean IsConsonant => value.ToString().IsMatch("['bcdfghjklmnpqrstvwxyz']; i");

   public IRangeItem Successor => (Char)(value + 1);

   public IRangeItem Predecessor => (Char)(value - 1);

   public Range Range() => new((Char)'a', this, false);

   public Int Ord => value;

   public IObject Find(string input, int startIndex, bool reverse)
   {
      var index = reverse ? input.LastIndexOf(value, startIndex) : input.IndexOf(value, startIndex);
      return index > -1 ? Some.Object((Int)index) : None.NoneValue;
   }

   public Tuple FindAll(string input) => new(input.FindAll(value.ToString()).Select(Int.IntObject).ToArray());

   public String Replace(string input, string replacement, bool reverse)
   {
      var index = reverse ? input.LastIndexOf(value) : input.IndexOf(value);

      if (index > -1)
      {
         return input.Keep(index) + replacement + input.Drop(index + 1);
      }
      else
      {
         return input;
      }
   }

   public String Replace(string input, Lambda lambda, bool reverse)
   {
      var index = reverse ? input.LastIndexOf(value) : input.IndexOf(value);

      if (index > -1)
      {
         var text = input.Drop(index);
         var length = text.Length;
         var replacement = lambda.Invoke((String)text, (Int)index, (Int)length);

         return input.Keep(index) + replacement + input.Drop(index + 1);
      }
      else
      {
         return input;
      }
   }

   public String ReplaceAll(string input, string replacement) => input.Replace(value.ToString(), replacement);

   public String ReplaceAll(string input, Lambda lambda)
   {
      var builder = new StringBuilder();
      var index = input.IndexOf(value);
      var start = 0;
      while (index > -1)
      {
         var replacement = lambda.Invoke((Char)value, (Int)index, Int.One);
         builder.Append(input.Drop(start));
         builder.Append(replacement.AsString);
         start = index + 1;
         index = input.IndexOf(value);
      }

      builder.Append(input.Drop(start));

      return builder.ToString();
   }

   public Tuple Split(string input) => new(input.Split(value).Select(String.StringObject).ToArray());

   public Tuple Partition(string input, bool reverse)
   {
      if (reverse)
      {
         var index = input.LastIndexOf(value);
         return index > -1 ? Tuple.Tuple3(input.Keep(index), value.ToString(), input.Drop(index + 1)) : Tuple.Tuple3(input, "", "");
      }
      else
      {
         if (input.Find(value.ToString()) is (true, var index))
         {
            return Tuple.Tuple3(input.Keep(index), value.ToString(), input.Drop(index + 1));
         }
         else
         {
            return Tuple.Tuple3(input, "", "");
         }
      }
   }

   public Int Count(string input)
   {
      var self = this;
      return input.Count(c => c == self.value);
   }

   public Int Count(string input, Lambda lambda)
   {
      var self = this;
      return input.Count(_ => lambda.Invoke(self).IsTrue);
   }

   public Byte Byte() => new((byte)value);
}