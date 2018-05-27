using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Packages
{
   public class SysClass : PackageClass
   {
      public override string Name => "Sys";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         registerPackageFunction("rand", (obj, msg) => function<Sys>(obj, sys => sys.Rand()));
         registerPackageFunction("println", (obj, msg) => function<Sys>(obj, sys => sys.Println(msg.Arguments)));
         registerPackageFunction("print", (obj, msg) => function<Sys>(obj, sys => sys.Print(msg.Arguments)));
         registerPackageFunction("put", (obj, msg) => function<Sys>(obj, sys => sys.Put(msg.Arguments)));
         registerPackageFunction("readln", (obj, msg) => function<Sys>(obj, sys => sys.Readln()));
         registerPackageFunction("peek", (obj, msg) => function<Sys>(obj, sys => sys.Peek(msg.Arguments[0])));
      }
   }
}