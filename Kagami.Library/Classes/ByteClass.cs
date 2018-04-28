using Kagami.Library.Objects;

namespace Kagami.Library.Classes
{
   public class ByteClass : BaseClass
   {
      public override string Name => "Byte";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         formatMessage<Byte>();
         compareMessages();
      }
   }
}