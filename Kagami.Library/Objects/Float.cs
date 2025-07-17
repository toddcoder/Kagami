using System.Numerics;
using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Objects;

public readonly struct Float : IObject, INumeric, IObjectCompare, IComparable<Float>, IEquatable<Float>, IFormattable, IComparable, IMessageNumber
{
   public static implicit operator Float(double value) => new(value);

   public static IObject FloatObject(double value) => new Float(value);

   public static IObject Zero => FloatObject(0.0);

   private readonly double value;

   public Float(double value) : this()
   {
      this.value = value;
   }

   public double Value => value;

   public string ClassName => "Float";

   public bool IsZero => value == 0;

   public bool IsPositive => value > 0;

   public bool IsNegative => value < 0;

   public bool IsPrimitive => true;

   public INumeric ToByte() => new KByte(AsByte());

   public byte AsByte() => (byte)value;

   public bool IsByte => false;

   public INumeric ToInt() => new Int(AsInt32());

   public int AsInt32() => (int)value;

   public bool IsInt => false;

   public INumeric ToFloat() => this;

   public double AsDouble() => value;

   public bool IsFloat => true;

   public INumeric ToLong() => (Long)AsBigInteger();

   public BigInteger AsBigInteger() => (BigInteger)value;

   public bool IsLong => false;

   public INumeric ToComplex() => (Complex)AsComplex();

   public System.Numerics.Complex AsComplex() => new(value, 0);

   public bool IsComplex => false;

   public INumeric ToRational() => (Rational)AsRational();

   public (BigInteger, BigInteger) AsRational()
   {
      var x = value;
      var maxValue = int.MaxValue;
      var m = new[] { new[] { 1L, 0L }, new[] { 0L, 1L } };

      long ai;
      while (m[1][0] * (ai = (long)x) + m[1][1] <= maxValue)
      {
         var t = m[0][0] * ai + m[0][1];
         m[0][1] = m[0][0];
         m[0][0] = t;
         t = m[1][0] * ai + m[1][1];
         m[1][1] = m[1][0];
         m[1][0] = t;
         if (x == ai)
         {
            break;
         }

         x = 1 / (x - ai);
         if (x > 0x7FFFFFFF)
         {
            break;
         }
      }

      return (m[0][0], m[1][0]);
   }

   public bool IsRational => false;

   public INumeric ToDecimal() => new KDecimal(AsDecimal());

   public decimal AsDecimal() => (decimal)value;

   public bool IsDecimal => false;

   public KString ZFill(int count) => zfill(AsString, count);

   public IObject Negate() => (Float)(-value);

   public IObject Sign() => (Float)Math.Sign(value);

   public IObject Raise(INumeric power) => raise(this, power);

   public IObject Remainder(INumeric other) => (Float)(value % other.AsDouble());

   public IObject Divide(INumeric other) => (Float)(value / other.AsDouble());

   public IObject DivRem(INumeric other) => (Float)Math.IEEERemainder(value, other.AsDouble());

   public IObject Add(INumeric other) => (Float)(value + other.AsDouble());

   public IObject Subtract(INumeric other) => (Float)(value - other.AsDouble());

   public IObject Multiply(INumeric other) => (Float)(value * other.AsDouble());

   public IObject Sin() => (Float)Math.Sin(value);

   public IObject Cos() => (Float)Math.Cos(value);

   public IObject Tan() => (Float)Math.Tan(value);

   public IObject Asin() => (Float)Math.Asin(value);

   public IObject Acos() => (Float)Math.Cos(value);

   public IObject Atan() => (Float)Math.Atan(value);

   public IObject Atan2(INumeric other) => (Float)Math.Atan2(value, other.AsDouble());

   public IObject Sinh() => (Float)Math.Sinh(value);

   public IObject Cosh() => (Float)Math.Cosh(value);

   public IObject Tanh() => (Float)Math.Tanh(value);

   public IObject Asinh() => (Float)Math.Asinh(value);

   public IObject Acosh() => (Float)Math.Acosh(value);

   public IObject Atanh() => (Float)Math.Atanh(value);

   public IObject Sqrt() => (Float)Math.Sqrt(value);

   public IObject Log() => (Float)Math.Log10(value);

   public IObject Ln() => (Float)Math.Log(value);

   public IObject Exp() => (Float)Math.Exp(value);

   public IObject Abs() => (Float)Math.Abs(value);

   public IObject Ceiling() => (Float)Math.Ceiling(value);

   public IObject Floor() => (Float)Math.Floor(value);

   public IObject Fraction() => (Float)(value - (int)value);

   public IObject Round(INumeric other) => (Float)Math.Round(value, other.AsInt32());

   public string AsString => value.ToString();

   public string Image => floatImage(value);

   public int Hash => value.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Float f && value == f.value;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => value != 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public (INumeric, INumeric) Compatible(INumeric obj) => obj.ClassName switch
   {
      "Int" => (this, obj.ToFloat()),
      "Float" => (this, obj.ToFloat()),
      "Byte" => (this, obj.ToFloat()),
      "Long" => (ToLong(), obj.ToLong()),
      "Complex" => (ToComplex(), obj.ToComplex()),
      "Rational" => (ToRational(), obj.ToRational()),
      "Decimal" => (this, obj.ToDecimal()),
      _ => (this, obj.ToFloat())
   };

   public int Compare(IObject obj) => compatibleCompare(this, obj);

   public IObject Object => this;

   public KBoolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

   public KBoolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

   public int CompareTo(Float other) => value.CompareTo(other.value);

   public bool Equals(Float other) => value.Equals(other.value);

   public override bool Equals(object? obj) => obj is Float f && Equals(f);

   public override int GetHashCode() => Hash;

   public int CompareTo(object? obj) => CompareTo((Float)obj!);

   public KString Format(string format) => value.FormatUsing<double>(format, d => d.ToString(format.Replace("@", "e")));

   public IObject Increment(int amount = 1) => new Float(value + amount);

   public IObject Decrement(int amount = 1) => new Float(value - amount);

   public IObject Increment(INumeric numeric) => FloatObject(value + numeric.AsDouble());

   public Float Rand(Random random) => random.NextDouble() * value;

   public Float Rand(Random random, Float max) => random.NextDouble() * (max.value - value) + value;
}