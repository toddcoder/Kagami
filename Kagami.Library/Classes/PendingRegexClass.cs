using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class PendingRegexClass : BaseClass
{
   public override string Name => "PendingRegex";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      messages["/(_<String>)"] = (obj, args) => function<PendingRegex, KString>(obj, args, (p, s) => p.Replace(s));
      messages["/(_<Lambda>)"] = (obj, args) => function<PendingRegex, Lambda>(obj, args, (p, l) => p.Replace(l));
   }

   public override IObject DefaultValue => throw noDefaultValue("PendingRegex");
}