using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Packages;

public class MathClass : PackageClass
{
   public override string Name => "Math";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      registerPackageFunction("sin(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Sin(n)));
      registerPackageFunction("cos(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Cos(n)));
      registerPackageFunction("tan(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Tan(n)));
      registerPackageFunction("asin(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Asin(n)));
      registerPackageFunction("acos(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Acos(n)));
      registerPackageFunction("atan(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Atan(n)));
      registerPackageFunction("atan2(_,_)", (obj, msg) => function<KMath, IObject, IObject>(obj, msg, (m, x, y) => m.Atan2(x, y)));
      registerPackageFunction("sinh(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Sinh(n)));
      registerPackageFunction("cosh(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Cosh(n)));
      registerPackageFunction("tanh(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Tanh(n)));
      registerPackageFunction("asinh(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Asinh(n)));
      registerPackageFunction("acosh(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Acosh(n)));
      registerPackageFunction("atanh(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Atanh(n)));
      registerPackageFunction("sqrt(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Sqrt(n)));
      registerPackageFunction("log(_<Number>)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Log(n)));
      registerPackageFunction("log(_<Number>,of:_<Int>)",
         (obj, msg) => function<KMath, IObject, IObject>(obj, msg, (m, n1, n2) => m.Log(n1, n2)));
      registerPackageFunction("ln(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Ln(n)));
      registerPackageFunction("exp(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Exp(n)));
      registerPackageFunction("sign(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Sign(n)));
      registerPackageFunction("abs(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Abs(n)));
      registerPackageFunction("floor(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Floor(n)));
      registerPackageFunction("ceil(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.Ceiling(n)));
      registerPackageFunction("round(_<Number>,_<Int>)",
         (obj, msg) => function<KMath, IObject, Int>(obj, msg, (m, n, i) => m.Round(n, i.Value)));
      registerPackageFunction("int(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.XInt(n)));
      registerPackageFunction("float(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.XFloat(n)));
      registerPackageFunction("byte(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.XByte(n)));
      registerPackageFunction("long(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.XLong(n)));
      registerPackageFunction("complex(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.XComplex(n)));
      registerPackageFunction("rational(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.XRational(n)));
      registerPackageFunction("decimal(_)", (obj, msg) => function<KMath, IObject>(obj, msg, (m, n) => m.XDecimal(n)));
      registerPackageFunction("hypot(_,_)", (obj, msg) => function<KMath, IObject, IObject>(obj, msg, (m, x, y) => m.Hypot(x, y)));
      registerPackageFunction("convert(toLong:_<String>,base:_<Int>)",
         (obj, msg) => function<KMath, KString, Int>(obj, msg, (m, s, i) => m.StringToLong(s.Value, i.Value)));
      registerPackageFunction("convert(toFloat:_<String>,base:_<Int>)",
         (obj, msg) => function<KMath, KString, Int>(obj, msg, (m, s, i) => m.StringToFloat(s.Value, i.Value)));
      registerPackageFunction("frexp(_)", (obj, msg) => function<KMath, Float>(obj, msg, (m, f) => m.Frexp(f.Value)));
      registerPackageFunction("pi".get(), (obj, _) => function<KMath>(obj, m => m.Pi));
      registerPackageFunction("e".get(), (obj, _) => function<KMath>(obj, m => m.E));
      registerPackageFunction("degrees(_)", (obj, msg) => function<KMath, Float>(obj, msg, (m, n) => m.Degrees(n.Value)));
      registerPackageFunction("radians(_)", (obj, msg) => function<KMath, Float>(obj, msg, (m, n) => m.Radians(n.Value)));
   }
}