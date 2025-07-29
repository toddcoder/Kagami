using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Classes;

public class PlaceholderClass : BaseClass
{
   public override string Name => "Placeholder";

   public override bool AssignCompatible(BaseClass otherClass) => true;

   public override IObject DefaultValue => throw noDefaultValue("Placeholder");
}