using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Classes;

public class ReferenceClass : BaseClass
{
   public override string Name => "Reference";

   public override bool AssignCompatible(BaseClass otherClass) => true;

   public override IObject DefaultValue => throw noDefaultValue("Reference");
}