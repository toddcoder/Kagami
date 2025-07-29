using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Classes;

public class MessageClass : BaseClass
{
   public override string Name => "Message";

   public override IObject DefaultValue => throw noDefaultValue("Message");
}