using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Classes;

public class UserObjectPlaceholderClass : BaseClass
{
   public override string Name => "UserObjectPlaceholder";

   public override IObject DefaultValue => throw noDefaultValue("UserObjectPlaceholder");
}