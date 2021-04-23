using Kagami.Library.Packages;
using Kagami.Library.Runtime;

namespace Kagami.Awk
{
   public class Awk : Package
   {
      public override string ClassName => "Awk";

      public override void LoadTypes(Module module)
      {
         module.RegisterClass(new AwkClass());
         module.RegisterClass(new AwkifierClass());
         module.RegisterClass(new AwkRecordClass());
      }

      public Awkifier Awkify(string source, bool asFile) => new(source, asFile);
   }
}