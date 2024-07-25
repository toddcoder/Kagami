using System;
using System.Numerics;
using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Objects
{
   public readonly struct Byte : IObject, INumeric, IObjectCompare, IComparable<Byte>, IEquatable<Byte>, IFormattable, IComparable
   {
      public static implicit operator Byte(byte value) => new(value);

      public static IObject ByteObject(byte value) => new Byte(value);

      private readonly byte value;

      public Byte(byte value) : this() => this.value = value;

      public byte Value => value;

      public (INumeric, INumeric) Compatible(INumeric obj) => obj.ClassName switch
      {
         "Int" => (ToInt(), obj.ToInt()),
         "Float" => (ToFloat(), obj.ToFloat()),
         "Byte" => (this, obj.ToByte()),
         _ => (this, obj.ToByte())
      };

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

      public System.Numerics.Complex AsComplex() => new(value, 0);

      public bool IsComplex => false;

      public INumeric ToRational() => (Rational)AsRational();

      public (BigInteger, BigInteger) AsRational() => (value, 1);

      public bool IsRational => false;

      public String ZFill(int count) => zfill(value.ToString(), count);

      public IObject Raise(INumeric power) => raise(this, power);

      public string AsString => ((char)value).ToString();

      public string Image => $"{value}b";

      public int Hash => value.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Byte b && value == b.value;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => value > 0;

      public int Compare(IObject obj) => compatibleCompare(this, obj);

      public IObject Object => this;

      public Boolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

      public Boolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

      public int CompareTo(Byte other) => value.CompareTo(other.value);

      public bool Equals(Byte other) => value == other.value;

      public override bool Equals(object obj) => obj is Byte b && Equals(b);

      public override int GetHashCode() => Hash;

      public int CompareTo(object obj) => CompareTo((Byte)obj);

      public String Format(string format) => value.FormatUsing<byte>(format, b => b.ToString(format));
   }
}