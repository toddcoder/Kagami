using Kagami.Library.Objects;
using Kagami.Library.Packages;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.IO
{
   public class IOClass : PackageClass
   {
      public override string Name => "IO";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         registerPackageFunction("File", (obj, msg) => function<IO, KString>(obj, msg, (io, path) => io.File(path.Value)));
         registerPackageFunction("Folder", (obj, msg) => function<IO, KString>(obj, msg, (io, path) => io.File(path.Value)));
      }
   }
}