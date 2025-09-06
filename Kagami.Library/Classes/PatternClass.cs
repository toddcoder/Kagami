using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class PatternClass : BaseClass
{
   public override string Name => "Pattern";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      messages["using(_<Dictionary>)"] = (obj, msg) => function<Pattern, Dictionary>(obj, msg, (p, d) => p.With(d));
   }

   public override IObject DefaultValue => throw noDefaultValue("Pattern");
}