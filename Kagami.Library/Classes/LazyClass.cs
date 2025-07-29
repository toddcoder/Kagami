using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Classes;

public class LazyClass : BaseClass
{
   public override string Name => "Lazy";

   IObject getValue(IObject obj) => ((Lazy)obj).Value;

   public override IObject DynamicInvoke(IObject obj, Message message) => SendMessage(getValue(obj), message);

   public override bool DynamicRespondsTo(Selector selector) => true;

   public override IObject DefaultValue => throw noDefaultValue("Lazy");
}