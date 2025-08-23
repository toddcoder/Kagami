using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class IndexClass : BaseClass
{
   public override string Name => "Index";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      registerMessage("start".get(), (obj, _) => function<KIndex>(obj, i => i.Start));
      registerMessage("end".get(), (obj, _) => function<KIndex>(obj, i => i.End));
      registerMessage("length".get(), (obj, _) => function<KIndex>(obj, i => i.Length));
      registerMessage("startSucc()", (obj, _) => function<KIndex>(obj, i => i.StartSucc()));
      registerMessage("startPred()", (obj, _) => function<KIndex>(obj, i => i.StartPred()));
      registerMessage("endSucc()", (obj, _) => function<KIndex>(obj, i => i.EndSucc()));
      registerMessage("endPred()", (obj, _) => function<KIndex>(obj, i => i.EndPred()));
      registerMessage("single()", (obj, _) => function<KIndex>(obj, i => i.Single()));
      registerMessage("range()", (obj, _) => function<KIndex>(obj, i => i.Range()));
      registerMessage("+(_)", (obj, msg) => function<KIndex, Int>(obj, msg, (i, c) => i.Shift(c.Value)));
      registerMessage("-(_)", (obj, msg) => function<KIndex, Int>(obj, msg, (i, c) => i.Shift(-c.Value)));
      registerMessage("*(_)", (obj, msg) => function<KIndex, Int>(obj, msg, (i, c) => i.Expand(c.Value)));
      registerMessage("/(_)", (obj, msg) => function<KIndex, Int>(obj, msg, (i, c) => i.Contract(c.Value)));
   }

   public override IObject DefaultValue => new KIndex(0, 0, 0);
}