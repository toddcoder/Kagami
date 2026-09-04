using System.Numerics;
using Core.Matching;
using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using Complex = Kagami.Library.Objects.Complex;

namespace Kagami.Library.Operations;

public static class NumericFunctions
{
   public static IObject apply(IObject x, IObject y, Func<int, int, Int> int32Func, Func<double, double, Float> doubleFunc,
      Func<byte, byte, Int> byteFunc, Func<IMessageNumber, INumeric, IObject> message, string messageName)
   {
      if (x is INumeric n1 && y is INumeric n2)
      {
         var (cx, cy) = n1.Compatible(n2);
         switch (cx.ClassName)
         {
            case "Int":
            {
               var ix = cx.AsInt32();
               var iy = cy.AsInt32();
               return int32Func(ix, iy);
            }
            case "Float":
            {
               var dx = cx.AsDouble();
               var dy = cy.AsDouble();
               return doubleFunc(dx, dy);
            }
            case "Byte":
            {
               var bx = cx.AsByte();
               var by = cy.AsByte();
               return byteFunc(bx, by);
            }
            default:

               if (cx is IMessageNumber mn)
               {
                  return message(mn, cy);
               }
               else
               {
                  return sendMessage(x, messageName + "(_)", y);
               }
         }
      }
      else
      {
         return sendMessage(x, messageName, y);
      }
   }

   public static IObject apply(INumeric x, INumeric y, Func<int, int, Int> int32Func, Func<double, double, Float> doubleFunc,
      Func<byte, byte, Int> byteFunc, Func<IMessageNumber, INumeric, IObject> message, string messageName)
   {
      var (cx, cy) = x.Compatible(y);
      switch (cx.ClassName)
      {
         case "Int":
            var ix = cx.AsInt32();
            var iy = cy.AsInt32();
            return int32Func(ix, iy);
         case "Float":
            var dx = cx.AsDouble();
            var dy = cy.AsDouble();
            return doubleFunc(dx, dy);
         case "Byte":
            var bx = cx.AsByte();
            var by = cy.AsByte();
            return byteFunc(bx, by);
         default:
            if (cx is IMessageNumber mn)
            {
               return message(mn, cy);
            }
            else
            {
               return sendMessage((IObject)x, messageName, (IObject)y);
            }
      }
   }

   public static IObject apply(IObject x, Func<int, Int> int32Func, Func<double, Float> doubleFunc, Func<byte, KByte> byteFunc,
      Func<IMessageNumber, IObject> message, string messageName)
   {
      if (x is INumeric n)
      {
         return x.ClassName switch
         {
            "Int" => int32Func(n.AsInt32()),
            "Float" => doubleFunc(n.AsDouble()),
            "Byte" => byteFunc(n.AsByte()),
            _ => x is IMessageNumber mn ? message(mn) : sendMessage(x, messageName)
         };
      }
      else
      {
         return sendMessage(x, messageName);
      }
   }

   public static IObject apply<T>(IObject x, Func<int, T> int32Func, Func<double, T> doubleFunc, Func<byte, T> byteFunc,
      Func<IMessageNumber, T> message, string messageName)
      where T : IObject
   {
      if (x is INumeric n)
      {
         return x.ClassName switch
         {
            "Int" => int32Func(n.AsInt32()),
            "Float" => doubleFunc(n.AsDouble()),
            "Byte" => byteFunc(n.AsByte()),
            _ => x is IMessageNumber mn ? message(mn) : sendMessage(x, messageName)
         };
      }
      else
      {
         return sendMessage(x, messageName);
      }
   }

   public static IObject function(IObject x, Message message, Func<INumeric, INumeric, IObject> func, string messageName)
   {
      if (x is INumeric nx)
      {
         var y = message.Arguments[0];
         if (y is INumeric ny)
         {
            return func(nx, ny);
         }
         else
         {
            throw notNumeric(y);
         }
      }
      else
      {
         return sendMessage(x, messageName, message.Arguments);
      }
   }

   public static IObject function(IObject x, Func<INumeric, IObject> func)
   {
      if (x is INumeric nx)
      {
         return func(nx);
      }
      else
      {
         throw notNumeric(x);
      }
   }

   public static IObject function(IObject x, Func<double, Float> func, Func<IMessageNumber, IObject> messageFunc) => x switch
   {
      IMessageNumber mn => messageFunc(mn),
      INumeric nx => func(nx.AsDouble()),
      _ => throw notNumeric(x)
   };

   public static IObject function(IObject x, Func<int, Int> int32Func, Func<double, Float> doubleFunc, Func<byte, KByte> byteFunc,
      Func<IMessageNumber, IObject> messageFunc, string message)
   {
      return x is INumeric ? apply(x, int32Func, doubleFunc, byteFunc, messageFunc, message) : sendMessage(x, message);
   }

   public static IObject function<T>(IObject x, Func<int, T> int32Func, Func<double, T> doubleFunc, Func<byte, T> byteFunc,
      Func<IMessageNumber, T> messageFunc, string message)
      where T : IObject
   {
      return x is INumeric ? apply(x, int32Func, doubleFunc, byteFunc, messageFunc, message) : sendMessage(x, message);
   }

   public static IObject function(IObject x, Message message, Func<int, int, Int> int32Func,
      Func<double, double, Float> doubleFunc, Func<byte, byte, Int> byteFunc,
      Func<IMessageNumber, INumeric, IObject> messageFunc, string messageName)
   {
      return function(x, message, (a, b) => apply(a, b, int32Func, doubleFunc, byteFunc, messageFunc, messageName), messageName);
   }

   public static IObject function(IObject x, Message message, Func<double, double, Float> func,
      Func<IMessageNumber, INumeric, IObject> messageFunc, string messageName)
   {
      switch (x)
      {
         case IMessageNumber mn:
            var y = (INumeric)message.Arguments[0];
            return messageFunc(mn, y);
         case INumeric:
            return function(x, message, (a, b) => func(a.AsDouble(), b.AsDouble()), messageName);
         default:
            return sendMessage(x, messageName, message.Arguments);
      }
   }

   public static IObject integerDivision(IObject x, Message message)
   {
      if (x is INumeric nx)
      {
         var y = message.Arguments[0];
         if (y is INumeric ny)
         {
            return (Int)(nx.AsInt32() / ny.AsInt32());
         }
         else
         {
            throw notNumeric(y);
         }
      }
      else
      {
         throw notNumeric(x);
      }
   }

   public static int compatibleCompare(IObject x, IObject y)
   {
      if (y is Infinity inf)
      {
         return inf.IsPositive ? -1 : 1;
      }
      else
      {
         switch (x)
         {
            case INumeric nx when y is INumeric ny:
               var (left, right) = nx.Compatible(ny);
               return left switch
               {
                  Int i => i.CompareTo((Int)right),
                  Float f => f.CompareTo((Float)right),
                  KByte b => b.CompareTo((KByte)right),
                  Long l => l.CompareTo((Long)right),
                  Rational r => r.CompareTo((Rational)right),
                  Complex c => c.CompareTo((Complex)right),
                  KDecimal d => d.CompareTo((KDecimal)right),
                  _ => throw incompatibleClasses(x is INumeric ? y : x, "Numeric")
               };
            case UserObject uo:
               return ((Int)sendMessage(uo, "<=>", y)).Value;
            default:
               throw incompatibleClasses(x is INumeric ? y : x, "Numeric");
         }
      }
   }

   public static string floatImage(double value)
   {
      if (double.IsNaN(value))
      {
         return "nan";
      }
      else if (double.IsPositiveInfinity(value))
      {
         return "inf";
      }
      else if (double.IsNegativeInfinity(value))
      {
         return "-inf";
      }
      else
      {
         var str = value.ToString("g");
         return str.IsMatch("['.Ee']") ? str : $"{value}.0";
      }
   }

   public static string decimalImage(decimal value)
   {
      var str = value.ToString("g");
      return str.IsMatch("['.Ee']") ? $"{str}d" : $"{value}.0d";
   }

   public static double Asinh(double x) => Math.Log(x + Math.Sqrt(x * x + 1));

   public static double Acosh(double x) => Math.Log(x + Math.Sqrt(x * x - 1));

   public static double Atanh(double x) => Math.Log((1 + x) / (1 - x)) / 2.0;

   public static bool isZero(IObject obj) => obj is INumeric { IsZero: true };

   public static IObject raise(INumeric x, INumeric y)
   {
      if (x.IsFloat || y.IsFloat)
      {
         return Float.FloatObject(Math.Pow(x.AsDouble(), y.AsDouble()));
      }
      else if (x.IsInt && y.IsInt)
      {
         var accum = 1;
         var amount = x.AsInt32();
         for (var i = 0; i < y.AsInt32(); i++)
         {
            accum *= amount;
         }

         return Int.IntObject(accum);
      }
      else
      {
         var accum = BigInteger.One;
         var amount = x.AsBigInteger();
         for (var i = 0; i < y.AsBigInteger(); i++)
         {
            accum *= amount;
         }

         return Long.LongObject(accum);
      }
   }

   public static INumeric zero<T>() where T : INumeric
   {
      if (typeof(T) == typeof(Int))
      {
         return (Int)0;
      }
      else if (typeof(T) == typeof(Float))
      {
         return (Float)0.0;
      }
      else if (typeof(T) == typeof(KByte))
      {
         return (KByte)0;
      }
      else if (typeof(T) == typeof(Long))
      {
         return new Long(0);
      }
      else if (typeof(T) == typeof(Rational))
      {
         return new Rational(0, 1);
      }
      else if (typeof(T) == typeof(Complex))
      {
         return new Complex(System.Numerics.Complex.Zero);
      }
      else if (typeof(T) == typeof(KDecimal))
      {
         return (KDecimal)0;
      }
      else
      {
         throw expectedType("Numeric");
      }
   }

   public static INumeric zero(Type type)
   {
      if (type == typeof(Int))
      {
         return (Int)0;
      }
      else if (type == typeof(Float))
      {
         return (Float)0.0;
      }
      else if (type == typeof(KByte))
      {
         return (KByte)0;
      }
      else if (type == typeof(Long))
      {
         return new Long(0);
      }
      else if (type == typeof(Rational))
      {
         return new Rational(0, 1);
      }
      else if (type == typeof(Complex))
      {
         return new Complex(System.Numerics.Complex.Zero);
      }
      else if (type == typeof(KDecimal))
      {
         return (KDecimal)0;
      }
      else
      {
         throw expectedType("Numeric");
      }
   }

   public static INumeric one<T>() where T : INumeric
   {
      if (typeof(T) == typeof(Int))
      {
         return (Int)1;
      }
      else if (typeof(T) == typeof(Float))
      {
         return (Float)1.0;
      }
      else if (typeof(T) == typeof(KByte))
      {
         return (KByte)1;
      }
      else if (typeof(T) == typeof(Long))
      {
         return new Long(1);
      }
      else if (typeof(T) == typeof(Rational))
      {
         return new Rational(1, 1);
      }
      else if (typeof(T) == typeof(Complex))
      {
         return new Complex(System.Numerics.Complex.One);
      }
      else if (typeof(T) == typeof(KDecimal))
      {
         return (KDecimal)1;
      }
      else
      {
         throw expectedType("Numeric");
      }
   }

   public static INumeric one(Type type)
   {
      if (type == typeof(Int))
      {
         return (Int)1;
      }
      else if (type == typeof(Float))
      {
         return (Float)1.0;
      }
      else if (type == typeof(KByte))
      {
         return (KByte)1;
      }
      else if (type == typeof(Long))
      {
         return new Long(1);
      }
      else if (type == typeof(Rational))
      {
         return new Rational(1, 1);
      }
      else if (type == typeof(Complex))
      {
         return new Complex(System.Numerics.Complex.One);
      }
      else if (type == typeof(KDecimal))
      {
         return (KDecimal)1;
      }
      else
      {
         throw expectedType("Numeric");
      }
   }

   private static BigInteger power(BigInteger x, BigInteger y, BigInteger p)
   {
      var r = new BigInteger(1);
      x %= p;

      while (y > 0)
      {
         if ((y & 1) == 1)
         {
            r = r * x % p;
         }

         y >>= 1;
         x = x * x % p;
      }

      return r;
   }

   private static bool millerTest(BigInteger d, BigInteger n)
   {
      var a = new BigInteger(new Random().Next(2, (int)n - 2));
      var x = power(a, d, n);
      if (x == 1 || x == n - 1)
      {
         return true;
      }

      while (d != n - 1)
      {
         x = x * x % n;
         d *= 2;
         if (x == 1)
         {
            return false;
         }

         if (x == n - 1)
         {
            return true;
         }
      }

      return false;
   }

   public static bool isPrime(INumeric numeric)
   {
      if (numeric.IsInt || numeric.IsLong || numeric.IsByte)
      {
         var n = numeric.AsBigInteger();
         if (n <= 1 || n == 4)
         {
            return false;
         }

         if (n <= 3)
         {
            return true;
         }

         var d = n - 1;

         while (d % 2 == 0)
         {
            d /= 2;
         }

         for (var i = 0; i < 5; i++)
         {
            if (!millerTest(d, n))
            {
               return false;
            }
         }

         return true;
      }

      return false;
   }
}