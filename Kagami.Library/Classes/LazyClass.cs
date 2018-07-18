using Kagami.Library.Objects;

namespace Kagami.Library.Classes
{
   public class LazyClass : BaseClass
   {
      public override string Name => "Lazy";

      IObject getValue(IObject obj) => ((Lazy)obj).Value;

      public override IObject DynamicInvoke(IObject obj, Message message) => SendMessage(getValue(obj), message);

      public override bool DynamicRespondsTo(string message) => true;
   }
}