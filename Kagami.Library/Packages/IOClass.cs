using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Packages;

public class IOClass : PackageClass
{
   public override string Name => "IO";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      registerPackageFunction("File(_)", (obj, msg) => function<IO, KString>(obj, msg, (io, path) => io.File(path.Value)));
      registerPackageFunction("Folder(_)", (obj, msg) => function<IO, KString>(obj, msg, (io, path) => io.Folder(path.Value)));
   }

   public override IObject DefaultValue => throw noDefaultValue("IO");
}