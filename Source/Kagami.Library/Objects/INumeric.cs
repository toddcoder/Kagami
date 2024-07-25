using System.Numerics;
using CComplex = System.Numerics.Complex;

namespace Kagami.Library.Objects
{
   public interface INumeric
   {
      (INumeric, INumeric) Compatible(INumeric obj);

      string ClassName { get; }

      bool IsZero { get; }

      bool IsPositive { get; }

      bool IsNegative { get; }

      bool IsPrimitive { get; }

      INumeric ToByte();

      byte AsByte();

      bool IsByte { get; }

      INumeric ToInt();

      int AsInt32();

      bool IsInt { get; }

      INumeric ToFloat();

      double AsDouble();

      bool IsFloat { get; }

      INumeric ToLong();

      BigInteger AsBigInteger();

      bool IsLong { get; }

      INumeric ToComplex();

      CComplex AsComplex();

      bool IsComplex { get; }

      INumeric ToRational();

      (BigInteger, BigInteger) AsRational();

      bool IsRational { get; }

      String ZFill(int count);

	   IObject Raise(INumeric power);
   }
}