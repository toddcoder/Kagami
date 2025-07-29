using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Classes;

public class TypeConstraintClass : BaseClass
{
   public override string Name => "TypeConstraint";

   public override IObject DefaultValue => throw noDefaultValue("TypeConstraint");
}