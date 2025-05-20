using System;
using System.Numerics;
using Core.Matching;
using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using Byte = Kagami.Library.Objects.Byte;
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

   public static IObject apply(IObject x, Func<int, Int> int32Func, Func<double, Float> doubleFunc, Func<byte, Byte> byteFunc,
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

   public static IObject function(IObject x, Func<int, Int> int32Func, Func<double, Float> doubleFunc, Func<byte, Byte> byteFunc,
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
                  Byte b => b.CompareTo((Byte)right),
                  Long l => l.CompareTo((Long)right),
                  Rational r => r.CompareTo((Rational)right),
                  Complex c => c.CompareTo((Complex)right),
                  _ => throw incompatibleClasses(x is INumeric ? y : x, "Numeric")
               };
            case UserObject uo:
               return ((Int)sendMessage(uo, "<=>", y)).Value;
            default:
               throw incompatibleClasses(x is INumeric ? y : x, "Numeric");
         }
      }
   }

   public static INumeric toNumeric(IObject obj) => obj is INumeric numeric ? numeric : throw incompatibleClasses(obj, "Numeric");

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
}