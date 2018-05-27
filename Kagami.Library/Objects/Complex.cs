using System;
using System.Numerics;
using Kagami.Library.Operations;
using Standard.Types.Booleans;
using Standard.Types.Collections;
using Standard.Types.Objects;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;
using CComplex = System.Numerics.Complex;

namespace Kagami.Library.Objects
{
   public struct Complex : IObject, INumeric, IObjectCompare, IComparable<Complex>, IEquatable<Complex>, IFormattable, IMessageNumber,
      IComparable
   {
      public static implicit operator Complex((double real, double imaginary) values) => new Complex(values.real, values.imaginary);

      public static implicit operator Complex(CComplex value) => new Complex(value);

      public static IObject Object((double real, double imaginary) values) => new Complex(values.real, values.imaginary);

      CComplex value;

      public Complex(CComplex value) : this() => this.value = value;

      public Complex(double real, double imaginary, bool isPolar = false) : this() => value = isPolar ? CComplex.FromPolarCoordinates(real, imaginary) : new CComplex(real, imaginary);

      public Complex(Float number) : this() => value = new CComplex(number.Value, 0.0);

      public Complex(double number): this() => value = new CComplex(0.0, number);

      public CComplex Value => value;

      public Float Real => value.Real;

      public Float Imaginary => value.Imaginary;

      public (INumeric, INumeric) Compatible(INumeric obj)
      {
         switch (obj.ClassName)
         {
            case "Int":
               return (this, obj.ToComplex());
            case "Float":
               return (this, obj.ToComplex());
            case "Byte":
               return (this, obj.ToComplex());
            case "Long":
               return (this, obj.ToComplex());
            case "Complex":
               return (this, obj.ToComplex());
            case "Rational":
               return (ToRational(), obj.ToRational());
            default:
               return (this, obj.ToComplex());
         }
      }

      public string ClassName => "Complex";

      public bool IsZero => value == CComplex.Zero;

      public bool IsPositive => value.Real > 0;

      public bool IsNegative => value.Real < 0;

      public bool IsPrimitive => false;

      public INumeric ToByte() => (Byte)AsByte();

      public byte AsByte() => (byte)value.Real;

      public bool IsByte => false;

      public INumeric ToInt() => (Int)AsInt32();

      public int AsInt32() => (int)value.Real;

      public bool IsInt => false;

      public INumeric ToFloat() => (Float)AsDouble();

      public double AsDouble() => value.Real;

      public bool IsFloat => false;

      public INumeric ToLong() => (Long)AsBigInteger();

      public BigInteger AsBigInteger() => (BigInteger)value.Real;

      public bool IsLong => false;

      public INumeric ToComplex() => this;

      public System.Numerics.Complex AsComplex() => value;

      public bool IsComplex => true;

      public INumeric ToRational() => ((Float)value.Real).ToRational();

      public (BigInteger, BigInteger) AsRational()
      {
         var rational = (Rational)ToRational();
         return (rational.Numerator, rational.Denominator);
      }

      public bool IsRational => false;

      public string AsString => $"{value.Real}{(value.Imaginary >= 0.0).Extend("+")}{value.Imaginary}i";

      public string Image => $"{floatImage(value.Real)}{(value.Imaginary >= 0.0).Extend("+")}{floatImage(value.Imaginary)}i";

      public int Hash => value.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Complex c && IsEqualTo(c);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => !IsZero;

      public int Compare(IObject obj) => compatibleCompare(this, obj);

      public int CompareTo(Complex other) => Math.Sign(value.Real*other.value.Real * value.Imaginary * other.value.Imaginary);

      public bool Equals(Complex other) => value.Equals(other.value);

      public String Format(string format)
      {
         return $"{value.Real.FormatAs(format)}{(value.Imaginary >= 0.0).Extend("+")}{value.Imaginary.FormatAs(format)}";
      }

      public IObject Negate() => (Complex)CComplex.Negate(value);

      public IObject Sign() => new Complex(Math.Sign(value.Real), Math.Sign(value.Imaginary));

      public IObject Raise(INumeric other) => (Complex)CComplex.Pow(value, other.AsComplex());

      public IObject Remainder(INumeric other) => (Complex)CComplex.Divide(value, other.AsComplex());

      public IObject Divide(INumeric other) => (Complex)CComplex.Divide(value, other.AsComplex());

      public IObject DivRem(INumeric other) => (Complex)CComplex.Divide(value, other.AsComplex());

      public IObject Add(INumeric other) => (Complex)CComplex.Add(value, other.AsComplex());

      public IObject Subtract(INumeric other) => (Complex)CComplex.Subtract(value, other.AsComplex());

      public IObject Multiply(INumeric other) => (Complex)CComplex.Multiply(value, other.AsComplex());

      public IObject Sin()=> (Complex)CComplex.Sin(value);

      public IObject Cos() => (Complex)CComplex.Cos(value);

      public IObject Tan() => (Complex)CComplex.Tan(value);

      public IObject Asin() => (Complex)CComplex.Asin(value);

      public IObject Acos() => (Complex)CComplex.Acos(value);

      public IObject Atan() => (Complex)CComplex.Atan(value);

      public IObject Atan2(INumeric other) => throw messageNotFound(classOf(this), "atan2");

      public IObject Sinh() => (Float)Math.Sinh(AsDouble());

      public IObject Cosh() => (Float)Math.Cosh(AsDouble());

      public IObject Tanh() => (Float)Math.Tanh(AsDouble());

      public IObject Asinh() => (Float)NumericFunctions.Asinh(AsDouble());

      public IObject Acosh() => (Float)NumericFunctions.Acosh(AsDouble());

      public IObject Atanh() => (Float)NumericFunctions.Atanh(AsDouble());

      public IObject Sqrt() => (Complex)CComplex.Sqrt(value);

      public IObject Log() => (Complex)CComplex.Log10(value);

      public IObject Ln() => (Complex)CComplex.Log(value);

      public IObject Exp() => (Complex)CComplex.Exp(value);

      public IObject Abs() => (Float)CComplex.Abs(value);

      public IObject Ceiling() => (Float)Math.Ceiling(AsDouble());

      public IObject Floor() => (Float)Math.Floor(AsDouble());

      public IObject Fraction() => (Float)(value.Real - (int)value.Real);

      public IObject Round(INumeric other) => (Float)Math.Round(AsDouble(), other.AsInt32());

      public int CompareTo(object obj) => CompareTo((Complex)obj);
   }
}
 