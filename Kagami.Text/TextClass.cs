using Kagami.Library;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Text
{
   public class TextClass : PackageClass
   {
      public override string Name => "Text";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         registerPackageFunction("StringBuffer", (obj, msg) => function<Text>(obj, t => t.StringBuffer()));
         registerPackageFunction("StringBuffer".Function("initialValue"),
            (obj, msg) => function<Text, String>(obj, msg, (t, s) => t.StringBuffer(s.Value)));
      }
   }
}