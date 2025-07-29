using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class UnitClass : BaseClass
{
   public override string Name => "Unit";

   public override IObject DefaultValue => KUnit.Value;
}