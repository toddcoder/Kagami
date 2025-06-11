using System.Numerics;
using Kagami.Library.Operations;
using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Objects;

public readonly struct Long : IObject, INumeric, IComparable<Long>, IEquatable<Long>, IRangeItem, IMessageNumber, IComparable
{
   public static implicit operator Long(BigInteger value) => new(value);

   public static IObject LongObject(BigInteger value) => new Long(value);

   private readonly BigInteger value;

   public Long(BigInteger value) : this() => this.value = value;

   public (INumeric, INumeric) Compatible(INumeric obj) => obj.ClassName switch
   {
      "Int" => (this, obj.ToLong()),
      "Float" => (this, obj.ToLong()),
      "Byte" => (this, obj.ToLong()),
      "Complex" => (ToComplex(), obj.ToComplex()),
      "Long" => (this, obj.ToLong()),
      "Rational" => (ToRational(), obj.ToRational()),
      "Decimal" => (this, obj.ToDecimal()),
      _ => (this, obj.ToLong())
   };

   public BigInteger Value => value;

   public string ClassName => "Long";

   public bool IsZero => value == BigInteger.Zero;

   public bool IsPositive => value > BigInteger.Zero;

   public bool IsNegative => value < BigInteger.Zero;

   public bool IsPrimitive => false;

   public INumeric ToByte() => (KByte)AsByte();

   public byte AsByte() => (byte)value;

   public bool IsByte => false;

   public INumeric ToInt() => (Int)AsInt32();

   public int AsInt32() => (int)value;

   public bool IsInt => false;

   public INumeric ToFloat() => (Float)AsDouble();

   public double AsDouble() => (double)value;

   public bool IsFloat => false;

   public INumeric ToLong() => this;

   public BigInteger AsBigInteger() => value;

   public bool IsLong => true;

   public INumeric ToComplex() => new Complex(AsComplex());

   public System.Numerics.Complex AsComplex() => new(AsDouble(), 0.0);

   public bool IsComplex => false;

   public INumeric ToRational() => new Rational(value, 1);

   public (BigInteger, BigInteger) AsRational() => (value, 1);

   public bool IsRational => false;

   public INumeric ToDecimal() => new XDecimal(AsDecimal());

   public decimal AsDecimal() => (decimal)value;

   public bool IsDecimal => false;

   public KString ZFill(int count) => zfill(AsString, count);

   public string AsString => value.ToString();

   public string Image => $"{value}L";

   public int Hash => value.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Long l && value == l.value;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => value != BigInteger.Zero;

   public int CompareTo(Long other) => value.CompareTo(other.value);

   public bool Equals(Long other) => value == other.value;

   public int Compare(IObject obj) => compatibleCompare(this, obj);

   public IObject Object => this;

   public KBoolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

   public KBoolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

   public IRangeItem Successor => (Long)(value + 1);

   public IRangeItem Predecessor => (Long)(value - 1);

   public KRange Range() => new(new Long(0), this, false);

   public IObject Negate() => (Long)(-value);

   public IObject Sign() => (Int)value.Sign;

   public IObject Raise(INumeric other) => (Long)BigInteger.Pow(value, other.AsInt32());

   public IObject Remainder(INumeric other) => (Long)(value % other.AsBigInteger());

   public IObject Divide(INumeric other) => (Long)(value / other.AsBigInteger());

   public IObject DivRem(INumeric other) => new KTuple(Divide(other), Remainder(other));

   public IObject Add(INumeric other) => (Long)(value + other.AsBigInteger());

   public IObject Subtract(INumeric other) => (Long)(value - other.AsBigInteger());

   public IObject Multiply(INumeric other) => (Long)(value * other.AsBigInteger());

   public IObject Sin() => (Float)Math.Sin(AsDouble());

   public IObject Cos() => (Float)Math.Cos(AsDouble());

   public IObject Tan() => (Float)Math.Tan(AsDouble());

   public IObject Asin() => (Float)Math.Asin(AsDouble());

   public IObject Acos() => (Float)Math.Acos(AsDouble());

   public IObject Atan() => (Float)Math.Atan(AsDouble());

   public IObject Atan2(INumeric other) => (Float)Math.Atan2(AsDouble(), other.AsDouble());

   public IObject Sinh() => (Float)Math.Sinh(AsDouble());

   public IObject Cosh() => (Float)Math.Cosh(AsDouble());

   public IObject Tanh() => (Float)Math.Tanh(AsDouble());

   public IObject Asinh() => (Float)NumericFunctions.Asinh(AsDouble());

   public IObject Acosh() => (Float)NumericFunctions.Acosh(AsDouble());

   public IObject Atanh() => (Float)NumericFunctions.Atanh(AsDouble());

   public IObject Sqrt() => (Float)Math.Sqrt(AsDouble());

   public IObject Log() => (Float)Math.Log10(AsDouble());

   public IObject Ln() => (Float)Math.Log(AsDouble());

   public IObject Exp() => (Float)Math.Exp(AsDouble());

   public IObject Abs() => (Long)BigInteger.Abs(value);

   public IObject Ceiling() => (IObject)ToFloat();

   public IObject Floor() => (IObject)ToFloat();

   public IObject Fraction() => (Float)0;

   public IObject Round(INumeric other) => (Float)Math.Round(AsDouble(), other.AsInt32());

   public int CompareTo(object? obj) => CompareTo((Long)obj!);

   public Long Factorial()
   {
      if (value <= 1)
      {
         return BigInteger.One;
      }
      else
      {
         var num = BigInteger.One;
         for (var index = 2; index <= value; index++)
         {
            num *= index;
         }

         return num;
      }
   }

   public IObject Increment(int amount = 1) => (Long)(value + amount);

   public IObject Decrement(int amount = 1) => (Long)(value - amount);
}