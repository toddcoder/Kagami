﻿using System;
using System.Numerics;
using Standard.Types.Collections;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Objects
{
   public struct Long : IObject, INumeric, IComparable<Long>, IEquatable<Long>, IRangeItem, IMessageNumber, IComparable
   {
      public static implicit operator Long(BigInteger value) => new Long(value);

      public static IObject Object(BigInteger value) => new Long(value);

      BigInteger value;

      public Long(BigInteger value) : this() => this.value = value;

      public (INumeric, INumeric) Compatible(INumeric obj)
      {
         switch (obj.ClassName)
         {
            case "Int":
               return (this, obj.ToLong());
            case "Float":
               return (this, obj.ToLong());
            case "Byte":
               return (this, obj.ToLong());
            case "Complex":
               return (ToComplex(), obj.ToComplex());
            case "Long":
               return (this, obj.ToLong());
            case "Rational":
               return (ToRational(), obj.ToRational());
            default:
               return (this, obj.ToLong());
         }
      }

      public BigInteger Value => value;

      public string ClassName => "Long";

      public bool IsZero => value == BigInteger.Zero;

      public bool IsPositive => value > BigInteger.Zero;

      public bool IsNegative => value < BigInteger.Zero;

      public bool IsPrimitive => false;

      public INumeric ToByte() => (Byte)AsByte();

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

      public System.Numerics.Complex AsComplex() => new System.Numerics.Complex(AsDouble(), 0.0);

      public bool IsComplex => false;

      public INumeric ToRational() => new Rational(value, 1);

      public (BigInteger, BigInteger) AsRational() => (value, 1);

      public bool IsRational => false;

      public string AsString => value.ToString();

      public string Image => $"{value}L";

      public int Hash => value.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Long l && value == l.value;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => value != BigInteger.Zero;

      public int CompareTo(Long other) => value.CompareTo(other.value);

      public bool Equals(Long other) => value == other.value;

      public int Compare(IObject obj) => compatibleCompare(this, obj);

      public IRangeItem Successor => (Long)(value + 1);

      public IRangeItem Predecessor => (Long)(value - 1);

      public Range Range() => new Range(new Long(0), this, false);

      public IObject Negate() => (Long)(-value);

      public IObject Sign() => (Int)value.Sign;

      public IObject Raise(INumeric other) => (Long)BigInteger.Pow(value, other.AsInt32());

      public IObject Remainder(INumeric other) => (Long)(value % other.AsBigInteger());

      public IObject Divide(INumeric other) => (Long)(value / other.AsBigInteger());

      public IObject DivRem(INumeric other) => new Tuple(Divide(other), Remainder(other));

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

      public IObject Sqrt() => (Float)Math.Sqrt(AsDouble());

      public IObject Log() => (Float)Math.Log10(AsDouble());

      public IObject Ln() => (Float)Math.Log(AsDouble());

      public IObject Exp() => (Float)Math.Exp(AsDouble());

      public IObject Abs() => (Long)BigInteger.Abs(value);

      public IObject Ceiling() => (IObject)ToFloat();

      public IObject Floor() => (IObject)ToFloat();

      public IObject Fraction() => (Float)0;

      public IObject Round(INumeric other) => (Float)Math.Round(AsDouble(), other.AsInt32());

      public int CompareTo(object obj) => CompareTo((Long)obj);
   }
}