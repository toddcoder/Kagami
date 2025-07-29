using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class NumericOpenRangeClass : BaseClass
{
   public override string Name => "NumericOpenRange";

   public override void RegisterMessages()
   {
      base.RegisterMessages();
      collectionMessages();
   }

   public override IObject DefaultValue => new NumericOpenRange((INumeric)Int.Zero, (INumeric)Int.One);
}