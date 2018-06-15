using System;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Operations.NumericFunctions;
using Byte = Kagami.Library.Objects.Byte;

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

      public IObject Atan2(IObject x, IObject y)
      {
         switch (x)
         {
            case INumeric nx when y is INumeric ny:
               return Float.FloatObject(System.Math.Atan2(nx.AsDouble(), ny.AsDouble()));
            case INumeric _:
               throw unableToConvert(y.AsString, "Float");
            default:
               throw unableToConvert(x.AsString, "Float");
         }
      }

      public IObject Sinh(IObject obj) => function(obj, d => System.Math.Sinh(d), n => n.Sin());

      public IObject Cosh(IObject obj) => function(obj, d => System.Math.Cos(d), n => n.Cos());

      public IObject Tanh(IObject obj) => function(obj, d => System.Math.Tan(d), n => n.Tan());

      public IObject Asinh(IObject obj) => function(obj, d => System.Math.Asin(d), n => n.Asin());

      public IObject Acosh(IObject obj) => function(obj, d => System.Math.Acos(d), n => n.Acos());

      public IObject Atanh(IObject obj) => function(obj, d => System.Math.Atan(d), n => n.Atan());

      public IObject Sqrt(IObject obj) => function(obj, d => System.Math.Sqrt(d), n => n.Sqrt());

      public IObject Log(IObject obj) => function(obj, d => System.Math.Log10(d), n => n.Log());

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
                  return Int.IntObject(-1);
               else if (n.IsZero)
                  return Int.IntObject(0);
               else
                  return Int.IntObject(1);
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
                  return Negate.Evaluate(n).Value;
               else
                  return obj;
            default:
               throw notNumeric(obj);
         }
      }

      public IObject Ceiling(IObject obj)
      {
         switch (obj)
         {
            case IMessageNumber mn:
               return mn.Ceiling();
            case INumeric n:
               return Float.FloatObject(System.Math.Ceiling(n.AsDouble()));
            default:
               throw notNumeric(obj);
         }
      }

      public IObject Floor(IObject obj)
      {
         switch (obj)
         {
            case IMessageNumber mn:
               return mn.Ceiling();
            case INumeric n:
               return Float.FloatObject(System.Math.Floor(n.AsDouble()));
            default:
               throw notNumeric(obj);
         }
      }

      public IObject Round(IObject obj, int size)
      {
         switch (obj)
         {
            case IMessageNumber mn:
               return mn.Round(new Int(size));
            case INumeric n:
               return Float.FloatObject(System.Math.Round(n.AsDouble(), size));
            default:
               throw notNumeric(obj);
         }
      }

      public T XConvert<T>(IObject obj, Func<INumeric, T> func)
         where T : INumeric
      {
         return obj is INumeric n ? func(n) : throw incompatibleClasses(obj, nameof(T));
      }

      public Int XInt(IObject source) => XConvert<Int>(source, n => n.AsInt32());

      public Float XFloat(IObject source) => XConvert<Float>(source, n => n.AsDouble());

      public Byte XByte(IObject source) => XConvert<Byte>(source, n => n.AsByte());

      public Long XLong(IObject source) => XConvert<Long>(source, n => n.AsBigInteger());

      public Complex XComplex(IObject source) => XConvert<Complex>(source, n => n.AsComplex());

      public Rational XRational(IObject source) => XConvert<Rational>(source, n => n.AsRational());
   }
}