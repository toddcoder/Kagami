using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Classes;

public class OpenRangeClass : BaseClass
{
   public override string Name => "OpenRange";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      collectionMessages();
   }

   public override IObject DefaultValue => throw noDefaultValue("OpenRange");
}