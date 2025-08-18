using System.Numerics;
using Core.Objects;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Operations.NumericFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Complex = Kagami.Library.Objects.Complex;

namespace Kagami.Library.Packages;

public class KMath : Package
{
   public KMath()
   {
      fields.New("pi", FieldType.Package, Float.FloatObject(Math.PI));
      fields.New("e", FieldType.Package, Float.FloatObject(Math.E));
      fields.New("i", FieldType.Package, Complex.ComplexObject((0, 1)));
      fields.New("tau", FieldType.Package, Float.FloatObject(Math.Tau));
   }

   public override string ClassName => "Math";

   public override void LoadTypes(Module module)
   {
      module.RegisterClass(new MathClass());
   }

   public IObject Sin(IObject obj) => function(obj, d => Math.Sin(d), n => n.Sin());

   public IObject Cos(IObject obj) => function(obj, d => Math.Cos(d), n => n.Cos());

   public IObject Tan(IObject obj) => function(obj, d => Math.Tan(d), n => n.Tan());

   public IObject Asin(IObject obj) => function(obj, d => Math.Asin(d), n => n.Asin());

   public IObject Acos(IObject obj) => function(obj, d => Math.Acos(d), n => n.Acos());

   public IObject Atan(IObject obj) => function(obj, d => Math.Atan(d), n => n.Atan());

   public IObject Atan2(IObject x, IObject y) => x switch
   {
      INumeric nx when y is INumeric ny => Float.FloatObject(Math.Atan2(nx.AsDouble(), ny.AsDouble())),
      INumeric => throw unableToConvert(y.AsString, "Float"),
      _ => throw unableToConvert(x.AsString, "Float")
   };

   public IObject Sinh(IObject obj) => function(obj, d => Math.Sinh(d), n => n.Sin());

   public IObject Cosh(IObject obj) => function(obj, d => Math.Cos(d), n => n.Cos());

   public IObject Tanh(IObject obj) => function(obj, d => Math.Tan(d), n => n.Tan());

   public IObject Asinh(IObject obj) => function(obj, d => Math.Asin(d), n => n.Asin());

   public IObject Acosh(IObject obj) => function(obj, d => Math.Acos(d), n => n.Acos());

   public IObject Atanh(IObject obj) => function(obj, d => Math.Atan(d), n => n.Atan());

   public IObject Sqrt(IObject obj) => function(obj, d => Math.Sqrt(d), n => n.Sqrt());

   public IObject Log(IObject obj) => function(obj, d => Math.Log10(d), n => n.Log());

   public IObject Log(IObject x, IObject y) => x switch
   {
      INumeric numeric when y is INumeric baseNumeric => Float.FloatObject(Math.Log(numeric.AsDouble(), baseNumeric.AsDouble())),
      INumeric => throw incompatibleClasses(y, "Float"),
      _ => throw incompatibleClasses(x, "Float")
   };

   public IObject Ln(IObject obj) => function(obj, d => Math.Log(d), n => n.Ln());

   public IObject Exp(IObject obj) => function(obj, d => Math.Exp(d), n => n.Exp());

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
               var _value = Negate.Evaluate(n);
               if (_value is (true, var value))
               {
                  return value;
               }
               else if (_value.Exception is (true, var exception))
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
      INumeric n => Float.FloatObject(Math.Ceiling(n.AsDouble())),
      _ => throw notNumeric(obj)
   };

   public IObject Floor(IObject obj) => obj switch
   {
      IMessageNumber mn => mn.Ceiling(),
      INumeric n => Float.FloatObject(Math.Floor(n.AsDouble())),
      _ => throw notNumeric(obj)
   };

   public IObject Round(IObject obj, int size) => obj switch
   {
      IMessageNumber mn => mn.Round(new Int(size)),
      INumeric n => Float.FloatObject(Math.Round(n.AsDouble(), size)),
      _ => throw notNumeric(obj)
   };

   public IObject Trunc(IObject obj) => obj switch
   {
      IMessageNumber mn => mn.Trunc(),
      INumeric n => Float.FloatObject(Math.Truncate(n.AsDouble())),
      _ => throw notNumeric(obj)
   };

   public T XConvert<T>(IObject obj, Func<INumeric, T> func) where T : IObject
   {
      var className = typeof(T).Name;
      switch (obj)
      {
         case INumeric n:
            return func(n);
         case KString s:
            var _baseClass = Module.Global.Value.Class(className);
            if (_baseClass is (true, var baseClass))
            {
               if (baseClass is IParse parse)
               {
                  return (T)parse.Parse(s.Value);
               }
               else
               {
                  throw fail($"Cannot convert to {className}");
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

   public KByte XByte(IObject source) => XConvert<KByte>(source, n => n.AsByte());

   public Long XLong(IObject source) => XConvert<Long>(source, n => n.AsBigInteger());

   public Complex XComplex(IObject source) => XConvert<Complex>(source, n => n.AsComplex());

   public Rational XRational(IObject source)
   {
      switch (source)
      {
         case INumeric numeric:
         {
            var (numerator, denominator) = numeric.AsRational();
            return new Rational(numerator, denominator);
         }
         case KString kString:
         {
            var _rational = RationalClass.Parse(kString.Value);
            if (_rational is (true, var rational))
            {
               return rational;
            }
            else
            {
               throw _rational.Exception;
            }
         }
         default:
            throw fail($"Can't convert {source.ClassName} to Rational");
      }
   }

   public KDecimal XDecimal(IObject source) => XConvert<KDecimal>(source, n => n.AsDecimal());

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

            return Float.FloatObject(Math.Sqrt(xDouble * xDouble + yDouble * yDouble));
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

   public KTuple Frexp(double number)
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

   protected static KTuple getFrexpResult(double mantissa, int exponent)
   {
      var m = Float.FloatObject(mantissa);
      var e = Int.IntObject(exponent);

      return new KTuple(m, e);
   }

   public Float Pi => (Float)fields["pi"];

   public Float E => (Float)fields["e"];

   public Complex I => (Complex)fields["i"];

   public Float Tau => (Float)fields["tau"];

   public Float Radians(double degrees) => Math.PI / 180 * degrees;

   public Float Degrees(double radians) => 180 / Math.PI * radians;

   public IObject Gcd(IObject a, IObject b)
   {
      return (a, b) switch
      {
         (Int ai, Int bi) => (Int)gcd(ai.AsInt32(), bi.AsInt32()),
         (Long al, Long bl) => (Long)bigGcd(al.AsBigInteger(), bl.AsBigInteger()),
         (Int ai, Long bl) => (Long)bigGcd(ai.AsBigInteger(), bl.AsBigInteger()),
         (Long al, Int bi) => (Long)bigGcd(al.AsBigInteger(), bi.AsBigInteger()),
         _ => throw incompatibleClasses(a, "Int or Long")
      };

      int gcd(int a, int b)
      {
         while (b != 0)
         {
            var temp = b;
            b = a % b;
            a = temp;
         }

         return Math.Abs(a);
      }

      BigInteger bigGcd(BigInteger a, BigInteger b)
      {
         while (b != 0)
         {
            var temp = b;
            b = a % b;
            a = temp;
         }

         return a < 0 ? -a : a;
      }
   }

   public IObject Lcm(IObject a, IObject b)
   {
      var g = Gcd(a, b);
      switch (g)
      {
         case Int gI:
         {
            var aI = ((INumeric)a).AsInt32();
            var bI = ((INumeric)b).AsInt32();
            var cI = aI * bI;
            var dI = cI / gI.Value;

            return (Int)dI;
         }
         case Long gL:
         {
            var aL = ((INumeric)a).AsBigInteger();
            var bL = ((INumeric)b).AsBigInteger();
            var cL = aL * bL;
            var dI = cL / gL.Value;

            return (Long)dI;
         }
         default:
            throw incompatibleClasses(g, "Int or Long");
      }
   }

   public Int IntFromString(KString kString) => kString.Value.Value().Int32();

   public Float FloatFromString(KString kString) => kString.Value.Value().Double();

   public KByte ByteFromString(KString kString) => kString.Value.Value().Byte();

   public Long LongFromString(KString kString) => BigInteger.Parse(kString.Value);

   public Complex ComplexFromString(KString kString) => System.Numerics.Complex.Parse(kString.Value, null);

   public Rational RationalFromString(KString kString)
   {
      var split = kString.Value.Split(' ');
      return new Rational(BigInteger.Parse(split[0]), BigInteger.Parse(split[1]));
   }

   public KDecimal DecimalFromString(KString kString) => kString.Value.Value().Decimal();

   public KArray Sieve(int n)
   {
      var isPrime = new bool[n];
      for (var i = 2; i < n; i++)
      {
         isPrime[i] = true;
      }

      for (var i = 2; i * i < n; i++)
      {
         if (isPrime[i])
         {
            for (var j = i * i; j < n; j += i)
            {
               isPrime[j] = false;
            }
         }
      }

      var primes = isPrime.Select((b, i) => b ? i : -1).Where(i => i != -1).Select(Int.IntObject);
      return new KArray(primes);
   }

   public KArray Factors(int number)
   {
      return new KArray(generateFactors(number).Select(Int.IntObject));

      IEnumerable<int> generateFactors(int number)
      {
         if (number <= 0)
         {
            throw fail("Number must be greater than 0");
         }

         var limit = (int)Math.Sqrt(number);
         var kArray = Sieve(limit);
         int[] primes = [.. kArray.List.Select(i => ((Int)i).Value)];
         foreach (var prime in primes)
         {
            while (number % prime == 0)
            {
               yield return prime;

               number /= prime;
            }
         }

         if (number > 1)
         {
            yield return number;
         }
      }
   }
}