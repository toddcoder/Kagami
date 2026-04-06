using Kagami.Library.Objects;
using Kagami.Library.Protocols;
using static Kagami.Library.Classes.ClassFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Classes;

public class ProtocolWrapperClass : BaseClass
{
   public override string Name => "ProtocolWrapper";

   public override IObject DefaultValue => new ProtocolWrapper(KUnit.Value, new Protocol(""));

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      registerMessage("object".get(), (obj, _) => function<ProtocolWrapper>(obj, p => p.Object));
   }

   public override IObject DynamicInvoke(IObject obj, Message message)
   {
      var target = ((ProtocolWrapper)obj).Object;
      return classOf(target).SendMessage(target, message);
   }
}