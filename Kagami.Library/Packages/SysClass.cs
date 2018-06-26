using Kagami.Library.Objects;
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
         registerPackageFunction("ticks", (obj, msg) => function<Sys>(obj, sys => sys.Ticks()));
         registerPackageFunction("fst", (obj, msg) => function<Sys, Tuple>(obj, msg, (sys, t) => sys.First(t)));
         registerPackageFunction("snd", (obj, msg) => function<Sys, Tuple>(obj, msg, (sys, t) => sys.Second(t)));
         registerPackageFunction("id".get(), (obj, msg) => function<Sys>(obj, sys => sys.ID));
      }
   }
}