using System;
using Kagami.Library.Objects;
using Standard.Types.RegularExpressions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using Byte = Kagami.Library.Objects.Byte;

namespace Kagami.Library.Operations
{
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
                     return message(mn, cy);
                  else
                     return sendMessage(x, messageName, y);
            }
         }
         else
            return sendMessage(x, messageName, y);
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
                  return message(mn, cy);
               else
                  return sendMessage((IObject)x, messageName, (IObject)y);
         }
      }

      public static IObject apply(IObject x, Func<int, Int> int32Func, Func<double, Float> doubleFunc, Func<byte, Byte> byteFunc,
         Func<IMessageNumber, IObject> message, string messageName)
      {
         if (x is INumeric n)
            switch (x.ClassName)
            {
               case "Int":
                  return int32Func(n.AsInt32());
               case "Float":
                  return doubleFunc(n.AsDouble());
               case "Byte":
                  return byteFunc(n.AsByte());
               default:
                  if (x is IMessageNumber mn)
                     return message(mn);
                  else
                     return sendMessage(x, messageName);
            }
         else
            return sendMessage(x, messageName);
      }

      public static IObject apply<T>(IObject x, Func<int, T> int32Func, Func<double, T> doubleFunc, Func<byte, T> byteFunc,
         Func<IMessageNumber, T> message, string messageName)
         where T : IObject
      {
         if (x is INumeric n)
            switch (x.ClassName)
            {
               case "Int":
                  return int32Func(n.AsInt32());
               case "Float":
                  return doubleFunc(n.AsDouble());
               case "Byte":
                  return byteFunc(n.AsByte());
               default:
                  if (x is IMessageNumber mn)
                     return message(mn);
                  else
                     return sendMessage(x, messageName);
            }
         else
            return sendMessage(x, messageName);
      }

      public static IObject function(IObject x, Message message, Func<INumeric, INumeric, IObject> func, string messageName)
      {
         if (x is INumeric nx)
         {
            var y = message.Arguments[0];
            if (y is INumeric ny)
               return func(nx, ny);
            else
               throw notNumeric(y);
         }
         else
            return sendMessage(x, messageName, message.Arguments);
      }

      public static IObject function(IObject x, Func<INumeric, IObject> func)
      {
         if (x is INumeric nx)
            return func(nx);
         else
            throw notNumeric(x);
      }

      public static IObject function(IObject x, Func<double, Float> func, Func<IMessageNumber, IObject> messageFunc)
      {
         switch (x)
         {
            case IMessageNumber mn:
               return messageFunc(mn);
            case INumeric nx:
               return func(nx.AsDouble());
            default:
               throw notNumeric(x);
         }
      }

      public static IObject function(IObject x, Func<int, Int> int32Func, Func<double, Float> doubleFunc, Func<byte, Byte> byteFunc,
         Func<IMessageNumber, IObject> messageFunc, string message)
      {
         if (x is INumeric)
            return apply(x, int32Func, doubleFunc, byteFunc, messageFunc, message);
         else
            return sendMessage(x, message);
      }

      public static IObject function<T>(IObject x, Func<int, T> int32Func, Func<double, T> doubleFunc, Func<byte, T> byteFunc,
         Func<IMessageNumber, T> messageFunc, string message)
         where T : IObject
      {
         if (x is INumeric)
            return apply(x, int32Func, doubleFunc, byteFunc, messageFunc, message);
         else
            return sendMessage(x, message);
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
            case INumeric _:
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
               return (Int)(nx.AsInt32() / ny.AsInt32());
            else
               throw notNumeric(y);
         }
         else
            throw notNumeric(x);
      }

      public static int compatibleCompare(IObject x, IObject y)
      {
         if (y is Infinity inf)
            return inf.IsPositive ? -1 : 1;
         else
            switch (x)
            {
               case INumeric nx when y is INumeric ny:
                  var (left, right) = nx.Compatible(ny);
                  switch (left)
                  {
                     case Int i:
                        return i.CompareTo((Int)right);
                     case Float f:
                        return f.CompareTo((Float)right);
                     case Byte b:
                        return b.CompareTo((Byte)right);
                     case Long l:
                        return l.CompareTo((Long)right);
                     case Rational r:
                        return r.CompareTo((Rational)right);
                     case Complex c:
                        return c.CompareTo((Complex)right);
                     default:
                        throw incompatibleClasses(x is INumeric ? y : x, "Numeric");
                  }
               case UserObject uo:
                  return ((Int)sendMessage(uo, "<=>", y)).Value;
               default:
                  throw incompatibleClasses(x is INumeric ? y : x, "Numeric");
            }
      }

      public static INumeric toNumeric(IObject obj) => obj is INumeric numeric ? numeric : throw incompatibleClasses(obj, "Numeric");

      public static string floatImage(double value)
      {
         if (double.IsNaN(value))
            return "nan";
         else if (double.IsPositiveInfinity(value))
            return "inf";
         else if (double.IsNegativeInfinity(value))
            return "-inf";
         else
         {
            var str = value.ToString("g");
            if (str.IsMatch("['.Ee']"))
               return str;
            else
               return $"{value}.0";
         }
      }

      public static double Asinh(double x) => Math.Log(x + Math.Sqrt(x * x + 1));

      public static double Acosh(double x)=> Math.Log(x + Math.Sqrt(x * x - 1));

      public static double Atanh(double x) => Math.Log((1 + x) / (1 - x)) / 2.0;

      public static bool isZero(IObject obj) => obj is INumeric n && n.IsZero;
   }
}