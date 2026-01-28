using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class SelectorClass : BaseClass
{
   public override string Name => "Selector";

   public override IObject DefaultValue => throw noDefaultValue("Selector");

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      registerMessage("name".get(), (obj, _) => function<Selector>(obj, s => KString.StringObject(s.Name)));
      registerMessage("items".get(), (obj, _) => function<Selector>(obj, s => s.GetSelectorItemArray()));
      registerMessage("assign(_<Lambda>)", (obj, msg) => function<Selector, Lambda>(obj, msg, (s, l) => s.Assign(l)));
   }
}