using System.Numerics;
using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;
using CComplex = System.Numerics.Complex;

namespace Kagami.Library.Objects;

public readonly struct XDecimal : IObject, INumeric, IObjectCompare, IComparable<XDecimal>, IEquatable<XDecimal>, IFormattable, IComparable, IMessageNumber
{
   public static implicit operator XDecimal(decimal value) => new(value);

   public static IObject XDecimalObject(decimal value) => new XDecimal(value);

   public static IObject Zero => XDecimalObject(0.0m);

   private readonly decimal value;

   public XDecimal(decimal value) : this()
   {
      this.value = value;
   }

   public decimal Value => value;

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

   public string ClassName => "Decimal";

   public string AsString => value.ToString();

   public string Image => decimalImage(value);

   public int Hash => value.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is XDecimal xDecimal && value == xDecimal.value;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => value != 0;

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

   public INumeric ToFloat() => new Float(AsDouble());

   public double AsDouble() => (double)value;

   public bool IsFloat => false;

   public INumeric ToLong() => new Long(AsBigInteger());

   public BigInteger AsBigInteger() => (BigInteger)value;

   public bool IsLong => false;

   public INumeric ToComplex() => new Complex(AsComplex());

   public CComplex AsComplex() => new(AsDouble(), 0);

   public bool IsComplex => false;

   public INumeric ToRational() => ToFloat().ToRational();

   public (BigInteger, BigInteger) AsRational() => ToFloat().AsRational();

   public bool IsRational => false;

   public INumeric ToDecimal() => this;

   public decimal AsDecimal() => value;

   public bool IsDecimal => true;

   public KString ZFill(int count) => zfill(AsString, count);

   public IObject Negate() => new XDecimal(-value);

   public IObject Sign() => value > 0 ? new XDecimal(1) : value < 0 ? new XDecimal(-1) : new XDecimal(0);

   public IObject Raise(INumeric power) => raise(this, power);

   public IObject Remainder(INumeric other) => new XDecimal(value % other.AsDecimal());

   public IObject Divide(INumeric other) => new XDecimal(value / other.AsDecimal());

   public IObject DivRem(INumeric other) => new KTuple(Divide(other), Remainder(other));

   public IObject Add(INumeric other) => new XDecimal(value + other.AsDecimal());

   public IObject Subtract(INumeric other) => new XDecimal(value - other.AsDecimal());

   public IObject Multiply(INumeric other) => new XDecimal(value * other.AsDecimal());

   public IObject Sin() => new Float(Math.Sin(AsDouble()));

   public IObject Cos() => new Float(Math.Cos(AsDouble()));

   public IObject Tan() => new Float(Math.Tan(AsDouble()));

   public IObject Asin() => new Float(Math.Asin(AsDouble()));

   public IObject Acos() => new Float(Math.Acos(AsDouble()));

   public IObject Atan() => new Float(Math.Atan(AsDouble()));

   public IObject Atan2(INumeric other) => new Float(Math.Atan2(AsDouble(), other.AsDouble()));

   public IObject Sinh() => new Float(Math.Sinh(AsDouble()));

   public IObject Cosh() => new Float(Math.Cosh(AsDouble()));

   public IObject Tanh() => new Float(Math.Tanh(AsDouble()));

   public IObject Asinh() => new Float(Math.Asinh(AsDouble()));

   public IObject Acosh() => new Float(Math.Acosh(AsDouble()));

   public IObject Atanh() => new Float(Math.Atanh(AsDouble()));

   public IObject Sqrt() => new Float(Math.Sqrt(AsDouble()));

   public IObject Log() => new Float(Math.Log(AsDouble()));

   public IObject Ln() => new Float(Math.Log(AsDouble()));

   public IObject Exp() => new Float(Math.Exp(AsDouble()));

   public IObject Abs() => new XDecimal(Math.Abs(value));

   public IObject Ceiling() => new XDecimal(Math.Ceiling(value));

   public IObject Floor() => new XDecimal(Math.Floor(value));

   public IObject Fraction() => new XDecimal(value - Math.Floor(value));

   public IObject Round(INumeric other) => new XDecimal(Math.Round(AsDecimal(), other.AsInt32()));

   public int Compare(IObject obj) => compatibleCompare(this, obj);

   public IObject Object => this;

   public KBoolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

   public KBoolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

   public int CompareTo(XDecimal other) => value.CompareTo(other.value);

   public bool Equals(XDecimal other) => value.Equals(other.value);

   public KString Format(string format) => value.FormatUsing<decimal>(format, d => d.ToString(format.Replace("@", "d")));

   public int CompareTo(object? obj) => CompareTo((XDecimal)obj!);
}