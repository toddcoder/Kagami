using Kagami.Library.Packages;
using Kagami.Library.Runtime;

namespace Kagami.IO
{
   public class IO : Package
   {
      public override string ClassName => "IO";

      public override void LoadTypes(Module module)
      {
         module.RegisterClass(new IOClass());
         module.RegisterClass(new FileClass());
      }

      public File File(string path) => new(path);

      public Folder Folder(string path) => new(path);
   }
}