using System.Numerics;
using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Objects;

public readonly struct Infinity : IObject, IObjectCompare, INumeric
{
   private readonly bool positive;

   public Infinity(bool positive) : this() => this.positive = positive;

   public (INumeric, INumeric) Compatible(INumeric obj) => (this, this);

   public string ClassName => "Infinity";

   public bool IsZero => false;

   public bool IsPositive => positive;

   public bool IsNegative => !positive;

   public bool IsPrimitive => false;

   public INumeric ToByte() => this;

   public byte AsByte() => 0;

   public bool IsByte => false;

   public INumeric ToInt() => this;

   public int AsInt32() => 0;

   public bool IsInt => false;

   public INumeric ToFloat() => this;

   public double AsDouble() => 0;

   public bool IsFloat => false;

   public INumeric ToLong() => this;

   public BigInteger AsBigInteger() => BigInteger.Zero;

   public bool IsLong => false;

   public INumeric ToComplex() => this;

   public System.Numerics.Complex AsComplex() => System.Numerics.Complex.Zero;

   public bool IsComplex => false;

   public INumeric ToRational() => this;

   public (BigInteger, BigInteger) AsRational() => (BigInteger.Zero, BigInteger.One);

   public bool IsRational => false;

   public INumeric ToDecimal() => this;

   public decimal AsDecimal() => 0;

   public bool IsDecimal => false;

   public KString ZFill(int count) => AsString;

   public IObject Raise(INumeric power) => raise(this, power);

   public string AsString => "_";

   public string Image => "_";

   public int Hash => ClassName.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Infinity inf && positive == inf.positive;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => true;

   public int Compare(IObject obj) => positive ? -1 : 1;

   public IObject Object => this;

   public KBoolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

   public KBoolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

   public IObject Increment(int amount = 1) => this;

   public IObject Decrement(int amount = 1) => this;
}