using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class SymbolClass : BaseClass
{
   public override string Name => "Symbol";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      registerMessage("name".get(), (obj, _) => function<SymbolObject>(obj, s => (KString)s.AsString));
   }

   public override IObject DefaultValue => new SymbolObject("");
}