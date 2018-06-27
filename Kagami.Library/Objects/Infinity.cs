using System.Numerics;
using Standard.Types.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct Infinity : IObject, IObjectCompare, INumeric
   {
      bool positive;

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

      public string AsString => "_";

      public string Image => "_";

      public int Hash => ClassName.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Infinity inf && positive == inf.positive;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => true;

      public int Compare(IObject obj) => positive ? -1 : 1;

      public IObject Object => this;

      public Boolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);
   }
}