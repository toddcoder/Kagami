using System.Numerics;
using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Objects;

public readonly struct KByte : IObject, INumeric, IObjectCompare, IComparable<KByte>, IEquatable<KByte>, IFormattable, IComparable
{
   public static implicit operator KByte(byte value) => new(value);

   public static IObject ByteObject(byte value) => new KByte(value);

   private readonly byte value;

   public KByte(byte value) : this() => this.value = value;

   public byte Value => value;

   public (INumeric, INumeric) Compatible(INumeric obj) => obj.ClassName switch
   {
      "Int" => (ToInt(), obj.ToInt()),
      "Float" => (ToFloat(), obj.ToFloat()),
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

   public INumeric ToDecimal() => new XDecimal(AsDecimal());

   public decimal AsDecimal() => value;

   public bool IsDecimal => false;

   public KString ZFill(int count) => zfill(value.ToString(), count);

   public IObject Raise(INumeric power) => raise(this, power);

   public string AsString => ((char)value).ToString();

   public string Image => $"{value}b";

   public int Hash => value.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is KByte b && value == b.value;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => value > 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public int Compare(IObject obj) => compatibleCompare(this, obj);

   public IObject Object => this;

   public KBoolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

   public KBoolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

   public int CompareTo(KByte other) => value.CompareTo(other.value);

   public bool Equals(KByte other) => value == other.value;

   public override bool Equals(object? obj) => obj is KByte b && Equals(b);

   public override int GetHashCode() => Hash;

   public int CompareTo(object? obj) => CompareTo((KByte)obj!);

   public KString Format(string format) => value.FormatUsing<byte>(format, b => b.ToString(format));

   public IObject Increment(int amount = 1) => (KByte)(value + amount);

   public IObject Decrement(int amount = 1) => (KByte)(value - amount);
}