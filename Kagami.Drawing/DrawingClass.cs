using Kagami.Library.Packages;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Drawing
{
   public class DrawingClass : PackageClass
   {
      public override string Name => "Drawing";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         registerPackageFunction("Point", (obj, msg) => function<Drawing>(obj, d => d.Point(msg.Arguments)));
      }
   }
}