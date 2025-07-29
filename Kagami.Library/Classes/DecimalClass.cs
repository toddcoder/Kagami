using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class DecimalClass : BaseClass, IParse, IEquivalentClass
{
   public override string Name => "Decimal";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      numericMessages();
      numericConversionMessages();
      rangeMessages();
      compareMessages();
   }

   public IObject Parse(string source) => new KDecimal(decimal.Parse(source.Replace("_", "").Replace("d", "")));

   public override bool IsNumeric => true;

   public override IObject DefaultValue => KDecimal.Zero;

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Number");
}