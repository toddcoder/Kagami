using System.Text;
using Core.Collections;
using Core.Matching;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct KChar : IObject, IComparable<KChar>, IEquatable<KChar>, IRangeItem, ITextFinding
{
   public static implicit operator KChar(char value) => new(value);

   public static IObject CharObject(char value) => new KChar(value);

   private readonly char value;

   public KChar(char value) : this() => this.value = value;

   public char Value => value;

   public string ClassName => "Char";

   public string AsString => value.ToString();

   public string Image => $"'{value}'";

   public int Hash => value.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is KChar c && value == c.value;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => value > 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public int Compare(IObject obj) => CompareTo((KChar)obj);

   public IObject Object => this;

   public KBoolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

   public KBoolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

   public int CompareTo(KChar other) => value.CompareTo(other.value);

   public bool Equals(KChar other) => value == other.value;

   public override bool Equals(object? obj) => obj is KChar c && Equals(c);

   public override int GetHashCode() => Hash;

   public KString Repeat(int count) => value.Repeat(count);

   public KChar Add(IObject c) => c switch
   {
      KChar ch => add(ch.value),
      Int count => add(count.Value),
      _ => throw incompatibleClasses(c, "Char or Int")
   };

   public KChar Subtract(IObject c) => c switch
   {
      KChar ch => subtract(ch.value),
      Int count => subtract(count.Value),
      _ => throw incompatibleClasses(c, "Char or Int")
   };

   public KChar add(int count) => (char)(value + count);

   public KChar add(char ch) => (char)(value + ch);

   public KChar subtract(int count) => (char)(value - count);

   public KChar subtract(char ch) => (char)(value - ch);

   public KChar Upper() => char.ToUpper(value);

   public KChar Lower() => char.ToLower(value);

   public KBoolean IsAlphaDigit => char.IsLetterOrDigit(value);

   public KBoolean IsAlpha => char.IsLetter(value);

   public KBoolean IsDigit => char.IsDigit(value);

   public KBoolean IsLower => char.IsLower(value);

   public KBoolean IsUpper => char.IsUpper(value);

   public KBoolean IsSpace => char.IsWhiteSpace(value);

   public KBoolean IsVowel => value.ToString().IsMatch("['aeiou']; i");

   public KBoolean IsConsonant => value.ToString().IsMatch("['bcdfghjklmnpqrstvwxyz']; i");

   public IRangeItem Successor => (KChar)(value + 1);

   public IRangeItem Predecessor => (KChar)(value - 1);

   public KRange Range() => new((KChar)'a', this, false);

   public Int Ord => value;

   public IObject Find(string input, int startIndex, bool reverse)
   {
      var index = reverse ? input.LastIndexOf(value, startIndex) : input.IndexOf(value, startIndex);
      return index > -1 ? Some.Object((Int)index) : None.NoneValue;
   }

   public KTuple FindAll(string input) => new(input.FindAll(value.ToString()).Select(Int.IntObject).ToArray());

   public KString Replace(string input, string replacement, bool reverse)
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

   public KString Replace(string input, Lambda lambda, bool reverse)
   {
      var index = reverse ? input.LastIndexOf(value) : input.IndexOf(value);

      if (index > -1)
      {
         var text = input.Drop(index);
         var length = text.Length;
         var replacement = lambda.Invoke((KString)text, (Int)index, (Int)length);

         return input.Keep(index) + replacement + input.Drop(index + 1);
      }
      else
      {
         return input;
      }
   }

   public KString ReplaceAll(string input, string replacement) => input.Replace(value.ToString(), replacement);

   public KString ReplaceAll(string input, Lambda lambda)
   {
      var builder = new StringBuilder();
      var index = input.IndexOf(value);
      var start = 0;
      while (index > -1)
      {
         var replacement = lambda.Invoke((KChar)value, (Int)index, Int.One);
         builder.Append(input.Drop(start));
         builder.Append(replacement.AsString);
         start = index + 1;
         index = input.IndexOf(value);
      }

      builder.Append(input.Drop(start));

      return builder.ToString();
   }

   public KTuple Split(string input) => new(input.Split(value).Select(KString.StringObject).ToArray());

   public KTuple Partition(string input, bool reverse)
   {
      if (reverse)
      {
         var index = input.LastIndexOf(value);
         return index > -1 ? KTuple.Tuple3(input.Keep(index), value.ToString(), input.Drop(index + 1)) : KTuple.Tuple3(input, "", "");
      }
      else
      {
         if (input.Find(value.ToString()) is (true, var index))
         {
            return KTuple.Tuple3(input.Keep(index), value.ToString(), input.Drop(index + 1));
         }
         else
         {
            return KTuple.Tuple3(input, "", "");
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

   public KByte Byte() => new((byte)value);

   public KChar Succ() => (KChar)(value + 1);

   public KChar Pred() => (KChar)(value - 1);
}