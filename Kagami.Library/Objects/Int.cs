using Core.Collections;
using Core.Dates.DateIncrements;
using System.Numerics;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Objects;

public readonly struct Int : IObject, INumeric, IComparable<Int>, IEquatable<Int>, IFormattable, IRangeItem, IComparable
{
   public static implicit operator Int(int value) => new(value);

   public static IObject IntObject(int value) => new Int(value);

   public static IObject Zero => IntObject(0);

   public static IObject One => IntObject(1);

   private readonly int value;

   public Int(int value) : this() => this.value = value;

   public int Value => value;

   public string ClassName => "Int";

   public bool IsZero => value == 0;

   public bool IsPositive => value > 0;

   public bool IsNegative => value < 0;

   public bool IsPrimitive => true;

   public string AsString => value.ToString();

   public string Image => value.ToString();

   public int Hash => value.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Int i && value == i.value;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => value != 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public (INumeric, INumeric) Compatible(INumeric obj) => obj.ClassName switch
   {
      "Int" => (this, obj.ToInt()),
      "Float" => (ToFloat(), obj.ToFloat()),
      "Byte" => (this, obj.ToByte()),
      "Long" => (ToLong(), obj.ToLong()),
      "Complex" => (ToComplex(), obj.ToComplex()),
      "Rational" => (ToRational(), obj.ToRational()),
      "Decimal" => (this, obj.ToDecimal()),
      _ => (this, obj.ToInt())
   };

   public INumeric ToByte() => new KByte(AsByte());

   public byte AsByte() => (byte)value;

   public bool IsByte => false;

   public INumeric ToInt() => this;

   public int AsInt32() => value;

   public bool IsInt => true;

   public INumeric ToFloat() => new Float(value);

   public double AsDouble() => value;

   public bool IsFloat => false;

   public INumeric ToLong() => new Long(value);

   public BigInteger AsBigInteger() => value;

   public bool IsLong => false;

   public INumeric ToComplex() => new Complex(AsComplex());

   public System.Numerics.Complex AsComplex() => new(value, 0);

   public bool IsComplex => false;

   public INumeric ToRational() => new Rational(AsRational());

   public (BigInteger, BigInteger) AsRational() => (value, 1);

   public bool IsRational => false;

   public INumeric ToDecimal() => new XDecimal(AsDecimal());

   public decimal AsDecimal() => value;

   public bool IsDecimal => false;

   public KString ZFill(int count) => zfill(AsString, count);

   public IObject Raise(INumeric power) => raise(this, power);

   public int Compare(IObject obj) => compatibleCompare(this, obj);

   public IObject Object => this;

   public KBoolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

   public KBoolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

   public int CompareTo(Int other) => value.CompareTo(other.value);

   public bool Equals(Int other) => value == other.value;

   public override bool Equals(object? obj) => obj is Int i && Equals(i);

   public override int GetHashCode() => Hash;

   public int CompareTo(object? obj) => CompareTo((Int)obj!);

   public KString Format(string format) => formatNumber(value, format);

   public KBoolean IsEven => value % 2 == 0;

   public KBoolean IsOdd => value % 2 != 0;

   private static bool isPrime(int value)
   {
      switch (value)
      {
         case < 2:
            return false;
         case 2 or 3:
            return true;
         default:
         {
            if (value % 2 == 0 || value % 3 == 0)
            {
               return false;
            }
            else
            {
               var limit = (int)Math.Sqrt(value);
               for (var i = 5; i <= limit; i += 6)
               {
                  if (value % i == 0 || value % (i + 2) == 0)
                  {
                     return false;
                  }
               }

               return true;
            }
         }
      }
   }

   public KBoolean IsPrime => isPrime(value);

   public Int Factorial()
   {
      if (value <= 1)
      {
         return 1;
      }
      else
      {
         var num = 1;
         for (var index = 2; index <= value; index++)
         {
            num *= index;
         }

         return num;
      }
   }

   public IRangeItem Successor => (Int)(value + 1);

   public IRangeItem Predecessor => (Int)(value - 1);

   public KRange Range() => new((Int)0, this, false);

   public Interval Millisecond => value.Millisecond();

   public Interval Second => value.Second();

   public Interval Minute => value.Minute();

   public Interval Hour => value.Hour();

   public Interval Day => value.Day();

   public Interval Week => (7 * value).Day();

   public KChar Char() => new((char)value);

   public KByte Byte() => new((byte)value);

   public IObject Times(Lambda lambda)
   {
      for (var i = 0; i < value; i++)
      {
         lambda.Invoke();
      }

      return KVoid.Value;
   }

   public Int ShiftLeft(IObject obj) => obj switch
   {
      Int i => value << i.value,
      KByte b => value << b.Value,
      Long l => value << (int)l.Value,
      _ => throw incompatibleClasses(obj, "Int, Byte or Long")
   };

   public Int ShiftRight(IObject obj) => obj switch
   {
      Int i => value >> i.value,
      KByte b => value >> b.Value,
      Long l => value >> (int)l.Value,
      _ => throw incompatibleClasses(obj, "Int, Byte or Long")
   };

   public Int NextPrime()
   {
      if (value < 2)
      {
         return 2;
      }

      var candidate = value + 1;
      while (!isPrime(candidate))
      {
         candidate++;
      }

      return candidate;
   }

   public IObject Increment(int amount = 1) => (Int)(value + amount);

   public IObject Decrement(int amount = 1) => (Int)(value - amount);

   public Int Max(Int other) => value > other.Value ? this : other;

   public Int Min(Int other) => value < other.Value ? this : other;
}