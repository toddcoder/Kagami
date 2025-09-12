using Core.Objects;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class FloatClass : BaseClass, IParse, IEquivalentClass
{
   protected Lazy<Random> random = new(() => new Random(DateTime.Now.Microsecond));

   public override string Name => "Float";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      numericMessages();
      numericConversionMessages();
      compareMessages();

      //messages["round(_<Int>)"] = (obj, msg) => function(obj, msg, (a, b) => Math.Round(a, (int)b), (a, b) => a.Round(b), "round");
      messages["rand()"] = (obj, _) => ((Float)obj).Rand(random.Value);
      messages["rand(_<Float>)"] = (obj, msg) => ((Float)obj).Rand(random.Value, (Float)msg.Arguments[0]);
      messages["isNan".get()] = (obj, _) => KBoolean.BooleanObject(double.IsNaN(((Float)obj).Value));
      messages["next()"] = (obj, _) => function<Float>(obj, f => f.Next());
      messages["prev()"] = (obj, _) => function<Float>(obj, f => f.Previous());
      messages["bits".get()] = (obj, _) => function<Float>(obj, f => f.Bits);

      messageNumberMessages();
   }

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      classMessages["e".get()] = (_, _) => (Float)Math.E;
      classMessages["pi".get()] = (_, _) => (Float)Math.PI;
      classMessages["nan".get()] = (_, _) => (Float)double.NaN;
      classMessages["parse(_)"] = (_, msg) => parse(msg.Arguments[0].AsString);
      classMessages["max".get()] = (_, _) => Float.FloatObject(double.MaxValue);
      classMessages["min".get()] = (_, _) => Float.FloatObject(double.MinValue);
      classMessages["rand()"] = (_, _) => (Float)random.Value.NextDouble();
   }

   public static IObject parse(string value)
   {
      try
      {
         var number = double.Parse(value.Replace("_", ""));
         return Success.Object(Float.FloatObject(number));
      }
      catch (Exception exception)
      {
         return Failure.Object(exception.Message);
      }
   }

   public IObject Parse(string source) => Float.FloatObject(source.Value().Double());

   public override bool IsNumeric => true;

   public override IObject DefaultValue => Float.Zero;

   public TypeConstraint EquivalentTypeConstraint() => TypeConstraint.FromList("Number");
}