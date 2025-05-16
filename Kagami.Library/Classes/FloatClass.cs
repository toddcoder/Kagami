using System;
using Core.Objects;
using Kagami.Library.Objects;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Classes;

public class FloatClass : BaseClass, IParse, IEquivalentClass
{
   public override string Name => "Float";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      numericMessages();
      numericConversionMessages();
      compareMessages();

      messages["round"] = (obj, msg) => function(obj, msg, (a, b) => Math.Round(a, (int)b), (a, b) => a.Round(b), "round");
   }

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      classMessages["e".get()] = (_, _) => (Float)Math.E;
      classMessages["pi".get()] = (_, _) => (Float)Math.PI;
      classMessages["nan".get()] = (_, _) => (Float)double.NaN;
      classMessages["parse"] = (_, msg) => parse(msg.Arguments[0].AsString);
      classMessages["max".get()] = (_, _) => Float.FloatObject(double.MaxValue);
      classMessages["min".get()] = (_, _) => Float.FloatObject(double.MinValue);
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

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Number");
}