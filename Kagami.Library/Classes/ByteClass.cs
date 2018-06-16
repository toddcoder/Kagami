using Kagami.Library.Objects;
using Standard.Types.Strings;

namespace Kagami.Library.Classes
{
   public class ByteClass : BaseClass, IParse
   {
      public override string Name => "Byte";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         formatMessage<Byte>();
         compareMessages();
      }

      public IObject Parse(string source) => Byte.ByteObject(source.ToByte());
   }
}