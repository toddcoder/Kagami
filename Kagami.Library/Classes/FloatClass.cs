using System;
using Kagami.Library.Objects;
using Standard.Types.Strings;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Classes
{
   public class FloatClass : BaseClass, IParse, IEquivalentClass
   {
      public override string Name => "Float";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         numericMessages();
         numericConversionMessages();
         formatMessage<Float>();
         compareMessages();

         messages["round"] = (obj, msg) => function(obj, msg, (a, b) => Math.Round(a, (int)b), (a, b) => a.Round(b), "round");
      }

      public override void RegisterClassMessages()
      {
         base.RegisterClassMessages();

         classMessages["e".get()] = (cls, msg) => (Float)Math.E;
         classMessages["pi".get()] = (cls, msg) => (Float)Math.PI;
         classMessages["nan".get()] = (cls, msg) => (Float)double.NaN;
         classMessages["parse"] = (cls, msg) => parse(msg.Arguments[0].AsString);
         classMessages["max".get()] = (cls, msg) => Float.FloatObject(double.MaxValue);
         classMessages["min".get()] = (cls, msg) => Float.FloatObject(double.MinValue);
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

      public IObject Parse(string source) => Float.FloatObject(source.ToDouble());

      public override bool IsNumeric => true;

      public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Number");
   }
}