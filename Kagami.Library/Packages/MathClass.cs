using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Packages
{
   public class MathClass : PackageClass
   {
      public override string Name => "Math";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         registerPackageFunction("sin", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Sin(n)));
         registerPackageFunction("cos", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Cos(n)));
         registerPackageFunction("tan", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Tan(n)));
         registerPackageFunction("asin", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Asin(n)));
         registerPackageFunction("acos", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Acos(n)));
         registerPackageFunction("atan", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Atan(n)));
         registerPackageFunction("atan2", (obj, msg) => function<Math, IObject, IObject>(obj, msg, (m, x, y) => m.Atan2(x, y)));
         registerPackageFunction("sinh", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Sinh(n)));
         registerPackageFunction("cosh", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Cosh(n)));
         registerPackageFunction("tanh", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Tanh(n)));
         registerPackageFunction("asinh", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Asinh(n)));
         registerPackageFunction("acosh", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Acosh(n)));
         registerPackageFunction("atanh", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Atanh(n)));
         registerPackageFunction("sqrt", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Sqrt(n)));
         registerPackageFunction("log", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Log(n)));
         registerPackageFunction("ln", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Ln(n)));
         registerPackageFunction("exp", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Exp(n)));
         registerPackageFunction("sign", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Sign(n)));
         registerPackageFunction("abs", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Abs(n)));
         registerPackageFunction("floor", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Floor(n)));
         registerPackageFunction("ceil", (obj, msg) => function<Math, IObject>(obj, msg, (m, n) => m.Ceiling(n)));
         registerPackageFunction("round", (obj, msg) => function<Math, IObject, Int>(obj, msg, (m, n, i) => m.Round(n, i.Value)));
      }
   }
}