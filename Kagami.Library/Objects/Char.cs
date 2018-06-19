using System;
using Standard.Types.Collections;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct Char : IObject, IComparable<Char>, IEquatable<Char>, IRangeItem
   {
      public static implicit operator Char(char value) => new Char(value);

      public static IObject CharObject(char value) => new Char(value);

      char value;

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

      public Boolean Between(IObject min, IObject max) => between(this, min, max);

      public int CompareTo(Char other) => value.CompareTo(other.value);

      public bool Equals(Char other) => value == other.value;

      public override bool Equals(object obj) => obj is Char c && Equals(c);

      public override int GetHashCode() => Hash;

      public String Repeat(int count) => value.Repeat(count);

      public Char Add(IObject c)
      {
         switch (c)
         {
            case Char ch:
               return add(ch.value);
            case Int count:
               return add(count.Value);
            default:
               throw incompatibleClasses(c, "Char or Int");
         }
      }

      public Char Subtract(IObject c)
      {
         switch (c)
         {
            case Char ch:
               return subtract(ch.value);
            case Int count:
               return subtract(count.Value);
            default:
               throw incompatibleClasses(c, "Char or Int");
         }
      }

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

      public Boolean IsVowel => value.ToString().IsMatch("['aeiou']", true);

      public Boolean IsConsonant => value.ToString().IsMatch("['bcdfghjklmnpqrstvwxyz']", true);

      public IRangeItem Successor => (Char)(value + 1);

      public IRangeItem Predecessor => (Char)(value - 1);

      public Range Range() => new Range((Char)'a', this, false);

      public Int Ord => value;
   }
}