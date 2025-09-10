using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class ComplexClass : BaseClass, IEquivalentClass
{
   public override string Name => "Complex";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      messageNumberMessages();
      compareMessages();

      messages["real".get()] = (obj, _) => function<Complex>(obj, c => c.Real);
      messages["imaginary".get()] = (obj, _) => function<Complex>(obj, c => c.Imaginary);
      messages["tuple()"] = (obj, _) => function<Complex>(obj, c => c.Tuple());
      messages["magnitude()"] = (obj, _) => function<Complex>(obj, c => c.Magnitude());
   }

   public override bool IsNumeric => true;

   public override IObject DefaultValue => new Complex(0, 0);

   public TypeConstraint EquivalentTypeConstraint() => TypeConstraint.FromList("Number");
}