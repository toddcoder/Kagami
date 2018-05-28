using Kagami.Library;
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

         registerPackageFunction("Point", (obj, msg) => function<Drawing>(obj, d => d.Point(msg.Arguments[0], msg.Arguments[0])));
         registerPackageFunction("Point".Function("size"), (obj, msg) => function<Drawing, Size>(obj, msg, (d, s) => d.Point(s)));
         registerPackageFunction("Size", (obj, msg) => function<Drawing>(obj, d => d.Size(msg.Arguments[0], msg.Arguments[0])));
         registerPackageFunction("Size".Function("point"), (obj, msg) => function<Drawing, Point>(obj, msg, (d, p) => d.Size(p)));
         registerPackageFunction("Rectangle".Function("x", "y", "width", "height"),
            (obj, msg) => function<Drawing>(obj,
               d => d.Rectangle(msg.Arguments[0], msg.Arguments[1], msg.Arguments[2], msg.Arguments[3])));
         registerPackageFunction("Rectangle", (obj, msg) => function<Drawing, Point, Size>(obj, msg, (d, p, s) => d.Rectangle(p, s)));

      }
   }
}