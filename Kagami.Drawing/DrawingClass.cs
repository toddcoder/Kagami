using Kagami.Library;
using Kagami.Library.Packages;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Drawing;

public class DrawingClass : PackageClass
{
   public override string Name => "Drawing";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      registerPackageFunction("Point", (obj, msg) => function<Drawing>(obj, d => d.Point(msg.Arguments[0], msg.Arguments[0])));
      registerPackageFunction("Point".Selector("size:<Size>"), (obj, msg) => function<Drawing, Size>(obj, msg, (d, s) => d.Point(s)));
      registerPackageFunction("Size", (obj, msg) => function<Drawing>(obj, d => d.Size(msg.Arguments[0], msg.Arguments[0])));
      registerPackageFunction("Size".Selector("point:<Point>"), (obj, msg) => function<Drawing, Point>(obj, msg, (d, p) => d.Size(p)));
      registerPackageFunction("Rectangle".Selector("x:<Number>", "y:<Number>", "width:<Number>", "height:<Number>"),
         (obj, msg) => function<Drawing>(obj,
            d => d.Rectangle(msg.Arguments[0], msg.Arguments[1], msg.Arguments[2], msg.Arguments[3])));
      registerPackageFunction("Rectangle", (obj, msg) => function<Drawing, Point, Size>(obj, msg, (d, p, s) => d.Rectangle(p, s)));

   }
}