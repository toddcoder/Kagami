using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class BeforeClass : BaseClass
{
   public override string Name => "Before";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      registerMessage("value".get(), (obj, _) => function<Before>(obj, b => b.Value));
   }

   public override IObject DefaultValue => new Before();
}