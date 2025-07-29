using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class VoidClass : BaseClass
{
   public override string Name => "Void";

   public override IObject DefaultValue => KVoid.Value;
}