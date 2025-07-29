using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class InfinityClass : BaseClass
{
   public override string Name => "Infinity";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      compareMessages();
   }

   public override IObject DefaultValue => new Infinity(true);
}