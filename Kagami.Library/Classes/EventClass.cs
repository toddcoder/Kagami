using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class EventClass : BaseClass
{
   public override string Name => "Event";

   public override IObject DefaultValue => new KEvent();

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      registerMessage("handler".get(), (obj, _) => function<KEvent>(obj, e => e.GetHandler()));
      registerMessage("handler".set(), (obj, msg) => function<KEvent, Lambda>(obj, msg, (e, l) => e.Handler = l));
      registerMessage("<<(_<Lambda>)", (obj, msg) => function<KEvent, Lambda>(obj, msg, (e, l) => e.SetHandler(l)));
      registerMessage("+(_<Lambda>)", (obj, msg) => function<KEvent, Lambda>(obj, msg, (e, l) => e.Add(l)));
      registerMessage("-(_<Lambda>)", (obj, msg) => function<KEvent, Lambda>(obj, msg, (e, l) => e.Remove(l)));
      registerMessage("invoke(_)", (obj, msg) => function<KEvent, IObject>(obj, msg, (e, o) => e.Invoke(o)));
   }
}