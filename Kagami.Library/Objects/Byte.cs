﻿using System;
using System.Numerics;
using Standard.Types.Collections;
using Standard.Types.Objects;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Objects
{
   public struct Byte : IObject, INumeric, IObjectCompare, IComparable<Byte>, IEquatable<Byte>, IFormattable, IComparable
   {
      public static implicit operator Byte(byte value) => new Byte(value);

      public static IObject Object(byte value) => new Byte(value);

      byte value;

      public Byte(byte value) : this() => this.value = value;

      public byte Value => value;

      public (INumeric, INumeric) Compatible(INumeric obj)
      {
         switch (obj.ClassName)
         {
            case "Int":
               return (ToInt(), obj.ToInt());
            case "Float":
               return (ToFloat(), obj.ToFloat());
            case "Byte":
               return (this, obj.ToByte());
            default:
               return (this, obj.ToByte());
         }
      }

      public string ClassName => "Byte";

      public bool IsZero => value == 0;

      public bool IsPositive => value > 0;

      public bool IsNegative => false;

      public bool IsPrimitive => true;

      public INumeric ToByte() => this;

      public byte AsByte() => value;

      public bool IsByte => true;

      public INumeric ToInt() => new Int(value);

      public int AsInt32() => value;

      public bool IsInt => false;

      public INumeric ToFloat() => new Float(value);

      public double AsDouble() => value;

      public bool IsFloat => false;

      public INumeric ToLong() => (Long)AsBigInteger();

      public BigInteger AsBigInteger() => value;

      public bool IsLong => false;

      public INumeric ToComplex() => (Complex)AsComplex();

      public System.Numerics.Complex AsComplex() => new System.Numerics.Complex(value, 0);

      public bool IsComplex => false;

      public INumeric ToRational() => (Rational)AsRational();

      public (BigInteger, BigInteger) AsRational() => (value, 1);

      public bool IsRational => false;

      public string AsString => ((char)value).ToString();

      public string Image => $"{value}b";

      public int Hash => value.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Byte b && value == b.value;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

	   public bool IsTrue => value > 0;

      public int Compare(IObject obj) => compatibleCompare(this, obj);

      public int CompareTo(Byte other) => value.CompareTo(other.value);

      public bool Equals(Byte other) => value == other.value;

      public override bool Equals(object obj) => obj is Byte b && Equals(b);

      public override int GetHashCode() => Hash;

      public int CompareTo(object obj) => CompareTo((Byte)obj);

      public String Format(string format) => value.FormatAs(format);
   }
}