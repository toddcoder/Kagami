using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class CoverClass : IteratorClass
{
   public override void RegisterMessages()
   {
      base.RegisterMessages();

      registerMessage("middle(_<Lambda>)", (obj, message) => ((Cover)obj).Middle((Lambda)message.Arguments[0]));
   }
}