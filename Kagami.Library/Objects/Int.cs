using System;
using System.Numerics;
using Standard.Types.Collections;
using Standard.Types.Dates.DateIncrements;
using Standard.Types.Objects;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Objects
{
   public struct Int : IObject, INumeric, IComparable<Int>, IEquatable<Int>, IFormattable, IRangeItem, IComparable
   {
      public static implicit operator Int(int value) => new Int(value);

      public static IObject IntObject(int value) => new Int(value);

      public static IObject Zero => IntObject(0);

      public static IObject One => IntObject(1);

      int value;

      public Int(int value) : this() => this.value = value;

      public int Value => value;

      public string ClassName => "Int";

      public bool IsZero => value == 0;

      public bool IsPositive => value > 0;

      public bool IsNegative => value < 0;

      public bool IsPrimitive => true;

      public string AsString => value.ToString();

      public string Image => value.ToString();

      public int Hash => value.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Int i && value == i.value;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => value != 0;

      public (INumeric, INumeric) Compatible(INumeric obj)
      {
         switch (obj.ClassName)
         {
            case "Int":
               return (this, obj.ToInt());
            case "Float":
               return (ToFloat(), obj.ToFloat());
            case "Byte":
               return (this, obj.ToByte());
            case "Long":
               return (ToLong(), obj.ToLong());
            case "Complex":
               return (ToComplex(), obj.ToComplex());
            case "Rational":
               return (ToRational(), obj.ToRational());
            default:
               return (this, obj.ToInt());
         }
      }

      public INumeric ToByte() => new Byte(AsByte());

      public byte AsByte() => (byte)value;

      public bool IsByte => false;

      public INumeric ToInt() => this;

      public int AsInt32() => value;

      public bool IsInt => true;

      public INumeric ToFloat() => new Float(value);

      public double AsDouble() => value;

      public bool IsFloat => false;

      public INumeric ToLong() => new Long(value);

      public BigInteger AsBigInteger() => value;

      public bool IsLong => false;

      public INumeric ToComplex() => new Complex(AsComplex());

      public System.Numerics.Complex AsComplex() => new System.Numerics.Complex(value, 0);

      public bool IsComplex => false;

      public INumeric ToRational() => new Rational(AsRational());

      public (BigInteger, BigInteger) AsRational() => (value, 1);

      public bool IsRational => false;

      public String ZFill(int count) => zfill(AsString, count);

      public int Compare(IObject obj) => compatibleCompare(this, obj);

      public IObject Object => this;

      public Boolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

      public Boolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

      public int CompareTo(Int other) => value.CompareTo(other.value);

      public bool Equals(Int other) => value == other.value;

      public override bool Equals(object obj) => obj is Int i && Equals(i);

      public override int GetHashCode() => Hash;

      public int CompareTo(object obj) => CompareTo((Int)obj);

      public String Format(string format)
      {
         if (format.StartsWith("b"))
            return Convert.ToString(value, 2);
         else if (format.StartsWith("o"))
            return Convert.ToString(value, 8);
         else
            return value.FormatAs(format);
      }

      public Boolean IsEven => value % 2 == 0;

      public Boolean IsOdd => value % 2 != 0;

      public Boolean IsPrime
      {
         get
         {
            if ((value & 1) == 0)
               return value == 2;
            else
            {
               var num = 3;
               while (num * num <= value)
               {
                  if (value % num == 0)
                     return false;

                  num += 2;
               }

               return value != 1;
            }
         }
      }

      public Int Factorial()
      {
         if (value <= 1)
            return 1;
         else
         {
            var num = 1;
            for (var index = 2; index <= value; index++)
               num *= index;
            return num;
         }
      }

      public IRangeItem Successor => (Int)(value + 1);

      public IRangeItem Predecessor => (Int)(value - 1);

      public Range Range() => new Range((Int)0, this, false);

      public Interval Millisecond => value.Millisecond();

      public Interval Second => value.Second();

      public Interval Minute => value.Minute();

      public Interval Hour => value.Hour();

      public Interval Day => value.Day();

      public Char Char() => new Char((char)value);
   }
}