using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class AnyClass : BaseClass
{
   public override string Name => "Any";

   public override IObject DefaultValue => new Any();
}