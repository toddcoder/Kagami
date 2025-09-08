using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class SlipClass : BaseClass
{
   public override string Name => "Slip";

   public override IObject DefaultValue => new Slip(Undefined.Value);
}