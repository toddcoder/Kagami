using Kagami.Library.Objects;
using Kagami.Library.Packages;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class PackageFunctionClass : BaseClass
{
   public override string Name => "PackageFunction";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      messages["invoke"] = (obj, msg) => function<PackageFunction>(obj, pf => pf.Invoke(msg.Arguments));
   }

   public override IObject DefaultValue => throw noDefaultValue("PackageFunction");
}