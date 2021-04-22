using System;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;
using Core.Exceptions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Operations.NumericFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Byte = Kagami.Library.Objects.Byte;
using String = Kagami.Library.Objects.String;
using Tuple = Kagami.Library.Objects.Tuple;

namespace Kagami.Library.Packages
{
   public class Math : Package
   {
      public Math()
      {
         fields.New("pi", Float.FloatObject(System.Math.PI));
         fields.New("e", Float.FloatObject(System.Math.E));
      }

      public override string ClassName => "Math";

      public override void LoadTypes(Module module)
      {
         module.RegisterClass(new MathClass());
      }

      public IObject Sin(IObject obj) => function(obj, d => System.Math.Sin(d), n => n.Sin());

      public IObject Cos(IObject obj) => function(obj, d => System.Math.Cos(d), n => n.Cos());

      public IObject Tan(IObject obj) => function(obj, d => System.Math.Tan(d), n => n.Tan());

      public IObject Asin(IObject obj) => function(obj, d => System.Math.Asin(d), n => n.Asin());

      public IObject Acos(IObject obj) => function(obj, d => System.Math.Acos(d), n => n.Acos());

      public IObject Atan(IObject obj) => function(obj, d => System.Math.Atan(d), n => n.Atan());

      public IObject Atan2(IObject x, IObject y) => x switch
      {
         INumeric nx when y is INumeric ny => Float.FloatObject(System.Math.Atan2(nx.AsDouble(), ny.AsDouble())),
         INumeric => throw unableToConvert(y.AsString, "Float"),
         _ => throw unableToConvert(x.AsString, "Float")
      };

      public IObject Sinh(IObject obj) => function(obj, d => System.Math.Sinh(d), n => n.Sin());

      public IObject Cosh(IObject obj) => function(obj, d => System.Math.Cos(d), n => n.Cos());

      public IObject Tanh(IObject obj) => function(obj, d => System.Math.Tan(d), n => n.Tan());

      public IObject Asinh(IObject obj) => function(obj, d => System.Math.Asin(d), n => n.Asin());

      public IObject Acosh(IObject obj) => function(obj, d => System.Math.Acos(d), n => n.Acos());

      public IObject Atanh(IObject obj) => function(obj, d => System.Math.Atan(d), n => n.Atan());

      public IObject Sqrt(IObject obj) => function(obj, d => System.Math.Sqrt(d), n => n.Sqrt());

      public IObject Log(IObject obj) => function(obj, d => System.Math.Log10(d), n => n.Log());

      public IObject Log(IObject x, IObject y) => x switch
      {
         INumeric numeric when y is INumeric baseNumeric => Float.FloatObject(System.Math.Log(numeric.AsDouble(), baseNumeric.AsDouble())),
         INumeric => throw incompatibleClasses(y, "Float"),
         _ => throw incompatibleClasses(x, "Float")
      };

      public IObject Ln(IObject obj) => function(obj, d => System.Math.Log(d), n => n.Ln());

      public IObject Exp(IObject obj) => function(obj, d => System.Math.Exp(d), n => n.Exp());

      public IObject Sign(IObject obj)
      {
         switch (obj)
         {
            case IMessageNumber mn:
               return mn.Sign();
            case INumeric n:
               if (n.IsNegative)
               {
                  return Int.IntObject(-1);
               }
               else if (n.IsZero)
               {
                  return Int.IntObject(0);
               }
               else
               {
                  return Int.IntObject(1);
               }

            default:
               throw notNumeric(obj);
         }
      }

      public IObject Abs(IObject obj)
      {
         switch (obj)
         {
            case IMessageNumber mn:
               return mn.Abs();
            case INumeric n:
               if (n.IsNegative)
               {
                  if (Negate.Evaluate(n).If(out var value, out var anyException))
                  {
                     return value;
                  }
                  else if (anyException.If(out var exception))
                  {
                     throw exception;
                  }
                  else
                  {
                     throw notNumeric(obj);
                  }
               }
               else
               {
                  return obj;
               }

            default:
               throw notNumeric(obj);
         }
      }

      public IObject Ceiling(IObject obj) => obj switch
      {
         IMessageNumber mn => mn.Ceiling(),
         INumeric n => Float.FloatObject(System.Math.Ceiling(n.AsDouble())),
         _ => throw notNumeric(obj)
      };

      public IObject Floor(IObject obj) => obj switch
      {
         IMessageNumber mn => mn.Ceiling(),
         INumeric n => Float.FloatObject(System.Math.Floor(n.AsDouble())),
         _ => throw notNumeric(obj)
      };

      public IObject Round(IObject obj, int size) => obj switch
      {
         IMessageNumber mn => mn.Round(new Int(size)),
         INumeric n => Float.FloatObject(System.Math.Round(n.AsDouble(), size)),
         _ => throw notNumeric(obj)
      };

      public T XConvert<T>(IObject obj, Func<INumeric, T> func) where T : IObject
      {
         var className = typeof(T).Name;
         switch (obj)
         {
            case INumeric n:
               return func(n);
            case String s:
               if (Module.Global.Class(className).If(out var baseClass))
               {
                  if (baseClass is IParse parse)
                  {
                     return (T)parse.Parse(s.Value);
                  }
                  else
                  {
                     throw $"Cannot convert to {className}".Throws();
                  }
               }
               else
               {
                  throw incompatibleClasses(obj, className);
               }

            default:
               throw incompatibleClasses(obj, className);
         }
      }

      public Int XInt(IObject source) => XConvert<Int>(source, n => n.AsInt32());

      public Float XFloat(IObject source) => XConvert<Float>(source, n => n.AsDouble());

      public Byte XByte(IObject source) => XConvert<Byte>(source, n => n.AsByte());

      public Long XLong(IObject source) => XConvert<Long>(source, n => n.AsBigInteger());

      public Complex XComplex(IObject source) => XConvert<Complex>(source, n => n.AsComplex());

      public Rational XRational(IObject source) => XConvert<Rational>(source, n => n.AsRational());

      public IObject Hypot(IObject x, IObject y)
      {
         switch (x)
         {
            case IMessageNumber xNumber when y is IMessageNumber yNumber:
               var two = (Int)2;
               var xSquared = (IMessageNumber)xNumber.Raise(two);
               var ySquared = (INumeric)yNumber.Raise(two);
               var sum = (IMessageNumber)xSquared.Add(ySquared);

               return sum.Sqrt();
            case INumeric xNumeric when y is INumeric yNumeric:
               var xDouble = xNumeric.AsDouble();
               var yDouble = yNumeric.AsDouble();

               return Float.FloatObject(System.Math.Sqrt(xDouble * xDouble + yDouble * yDouble));
            default:
               throw incompatibleClasses(x, "Number");
         }
      }

      public Long StringToLong(string value, int baseNum)
      {
         return new(convert(value.Replace("_", ""), baseNum, "0123456789abcdefghijklmnopqrstuvwxyz"));
      }

      public Float StringToFloat(string value, int baseNum)
      {
         return new(convertFloat(value.Replace("_", ""), baseNum, "0123456789abcdefghijklmnopqrstuvwxyz"));
      }

      public Tuple Frexp(double number)
      {
         var bits = BitConverter.DoubleToInt64Bits(number);
         if (double.IsNaN(number) || number + number == number || double.IsInfinity(number))
         {
            return getFrexpResult(number, 0);
         }

         var negative = bits < 0;
         var exponent = (int)(bits >> 52 & 0x7ffL);
         var mantissa = bits & 0xfffffffffffffL;
         if (exponent == 0)
         {
            exponent++;
         }
         else
         {
            mantissa |= 1L << 52;
         }

         exponent -= 1075;
         double realMantissa = mantissa;

         while (realMantissa > 1.0)
         {
            mantissa >>= 1;
            realMantissa /= 2.0;
            exponent++;
         }

         if (negative)
         {
            realMantissa *= -1;
         }

         return getFrexpResult(realMantissa, exponent);
      }

      protected static Tuple getFrexpResult(double mantissa, int exponent)
      {
         var m = Float.FloatObject(mantissa);
         var e = Int.IntObject(exponent);

         return new Tuple(m, e);
      }

      public Float Pi => (Float)fields["pi"];

      public Float E => (Float)fields["e"];

      public Float Radians(double degrees) => System.Math.PI / 180 * degrees;

      public Float Degrees(double radians) => 180 / System.Math.PI * radians;
   }
}