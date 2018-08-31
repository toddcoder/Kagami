using System;
using System.Numerics;
using Kagami.Library.Operations;
using Standard.Types.Collections;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Objects
{
   public struct Rational : IObject, INumeric, IRangeItem, IComparable<Rational>, IFormattable, IMessageNumber, IComparable
   {
      public static implicit operator Rational((BigInteger numerator, BigInteger denominator) values) => new Rational(values);

      public static IObject RationalObject((BigInteger numerator, BigInteger denominator) values) => new Rational(values);

      public static implicit operator Rational(Float value) => (Rational)value.ToRational();

      public static implicit operator Rational(double value) => (Rational)((Float)value).ToRational();

      static BigInteger gcd(BigInteger a, BigInteger b)
      {
         if (b != BigInteger.Zero)
            return gcd(b, a % b);
         else
            return a;
      }

      BigInteger numerator;
      BigInteger denominator;

      public Rational(BigInteger numerator, BigInteger denominator) : this() => setValues(numerator, denominator);

      public Rational((BigInteger numerator, BigInteger denominator) value) : this() => setValues(value.numerator, value.denominator);

      void setValues(BigInteger numerator, BigInteger denominator)
      {
         var result = gcd(BigInteger.Abs(numerator), BigInteger.Abs(denominator));
         this.numerator = numerator / result;
         this.denominator = denominator / result;
         this.numerator = this.numerator.Sign / this.denominator.Sign * BigInteger.Abs(this.numerator);
         this.denominator = BigInteger.Abs(this.denominator);
      }

      public (INumeric, INumeric) Compatible(INumeric obj) => (this, obj.ToRational());

      public BigInteger Numerator => numerator;

      public BigInteger Denominator => denominator;

      public string ClassName => "Rational";

      public bool IsZero => numerator == 0;

      public bool IsPositive => numerator > 0;

      public bool IsNegative => numerator < 0;

      public bool IsPrimitive => false;

      public INumeric ToByte() => (Byte)AsByte();

      public byte AsByte() => (byte)AsDouble();

      public bool IsByte => false;

      public INumeric ToInt() => (Int)AsInt32();

      public int AsInt32() => (int)AsDouble();

      public bool IsInt => false;

      public INumeric ToFloat() => (Float)AsDouble();

      public double AsDouble() => (double)numerator / (double)denominator;

      public bool IsFloat => false;

      public INumeric ToLong() => (Long)AsBigInteger();

      public BigInteger AsBigInteger() => numerator / denominator;

      public bool IsLong => false;

      public INumeric ToComplex() => (Complex)AsComplex();

      public System.Numerics.Complex AsComplex() => new System.Numerics.Complex(AsDouble(), 0);

      public bool IsComplex => false;

      public INumeric ToRational() => this;

      public (BigInteger, BigInteger) AsRational() => (numerator, denominator);

      public bool IsRational => true;

      public String ZFill(int count) => $"{zfill(numerator.ToString(), count)}/{zfill(denominator.ToString(), count)}";

      public string AsString => $"{numerator}/{denominator}";

      public string Image => AsString;

      public int Hash => (numerator.GetHashCode() + denominator.GetHashCode()).GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Rational r && numerator == r.numerator && denominator == r.denominator;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => numerator != 0;

      public int Compare(IObject obj) => compatibleCompare(this, obj);

      public IObject Object => this;

      public Boolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

      public Boolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

      public IRangeItem Successor => (IRangeItem)Add((Rational)(BigInteger.One, BigInteger.Zero));

      public IRangeItem Predecessor => (IRangeItem)Subtract((Rational)(BigInteger.One, BigInteger.Zero));

      public Range Range() => new Range((Rational)0.0, this, false);

      public int CompareTo(Rational other) => (numerator * other.denominator - other.numerator * denominator).Sign;

      public String Format(string format) => $"{numerator.ToString(format)}/{denominator.ToString(format)}";

	   public IObject Negate() => (Rational)(-numerator, denominator);

      public IObject Sign() => (Rational)(numerator.Sign, denominator.Sign);

      public IObject Raise(INumeric other)
      {
         var rhs = other.AsRational();
         var (n, d) = rhs;
         return (Rational)(BigInteger.Pow(numerator, (int)n), BigInteger.Pow(denominator, (int)d));
      }

      public IObject Remainder(INumeric other) => ((Rational)(1, 0)).Subtract((INumeric)Divide(other));

      public IObject Divide(INumeric other)
      {
         var rhs = other.AsRational();
         var (n, d) = rhs;
         return (Rational)(numerator * d, denominator * n);
      }

      public IObject DivRem(INumeric other) => new Tuple(Divide(other), Remainder(other));

      public IObject Add(INumeric other)
      {
         var rhs = other.AsRational();
         var (n, d) = rhs;
         return (Rational)(numerator * d + n * denominator, denominator * d);
      }

      public IObject Subtract(INumeric other)
      {
         var rhs = other.AsRational();
         var (n, d) = rhs;
         return (Rational)(numerator * d - n * denominator, denominator * d);
      }

      public IObject Multiply(INumeric other)
      {
         var rhs = other.AsRational();
         var (n, d) = rhs;
         return (Rational)(numerator * n, denominator * d);
      }

      public IObject Sin() => (Rational)Math.Sin(AsDouble());

      public IObject Cos() => (Rational)Math.Cos(AsDouble());

      public IObject Tan() => (Rational)Math.Tan(AsDouble());

      public IObject Asin() => (Rational)Math.Asin(AsDouble());

      public IObject Acos() => (Rational)Math.Acos(AsDouble());

      public IObject Atan() => (Rational)Math.Atan(AsDouble());

      public IObject Atan2(INumeric other) => (Rational)Math.Atan2(AsDouble(), other.AsDouble());

      public IObject Sinh() => (Float)Math.Sinh(AsDouble());

      public IObject Cosh() => (Float)Math.Cosh(AsDouble());

      public IObject Tanh() => (Float)Math.Tanh(AsDouble());

      public IObject Asinh() => (Float)NumericFunctions.Asinh(AsDouble());

      public IObject Acosh() => (Float)NumericFunctions.Acosh(AsDouble());

      public IObject Atanh() => (Float)NumericFunctions.Atanh(AsDouble());

      public IObject Sqrt() => (Rational)Math.Sqrt(AsDouble());

      public IObject Log() => (Rational)Math.Log10(AsDouble());

      public IObject Ln() => (Rational)Math.Log(AsDouble());

      public IObject Exp() => (Rational)Math.Exp(AsDouble());

      public IObject Abs() => (Rational)(BigInteger.Abs(numerator), BigInteger.Abs(denominator));

      public IObject Ceiling() => (Float)Math.Ceiling((double)numerator / (double)numerator);

      public IObject Floor() => (Float)Math.Floor((double)numerator / (double)numerator);

      public IObject Fraction() => (Float)(1f / (double)numerator);

      public IObject Round(INumeric other) => (Float)Math.Round(AsDouble(), other.AsInt32());

      public int CompareTo(object obj) => CompareTo((Rational)obj);
   }
}