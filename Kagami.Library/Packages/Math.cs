using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Packages
{
   public class Math : Package
   {
      public Math()
      {
         fields.New("pi", Float.Object(System.Math.PI));
         fields.New("e", Float.Object(System.Math.E));
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
               return Float.Object(System.Math.Atan2(nx.AsDouble(), ny.AsDouble()));
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
                  return Int.Object(-1);
               else if (n.IsZero)
                  return Int.Object(0);
               else
                  return Int.Object(1);
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
               return Float.Object(System.Math.Ceiling(n.AsDouble()));
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
               return Float.Object(System.Math.Floor(n.AsDouble()));
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
               return Float.Object(System.Math.Round(n.AsDouble(), size));
            default:
               throw notNumeric(obj);
         }
      }
   }
}