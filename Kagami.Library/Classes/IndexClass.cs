using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class IndexClass : BaseClass
{
   public override string Name => "Index";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      registerMessage("skipCount".get(), (obj, _) => function<KIndex>(obj, i => i.SkipCount));
      registerMessage("takeCount".get(), (obj, _) => function<KIndex>(obj, i => i.TakeCount));
   }
}