using Kagami.Library;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Drawing
{
   public class RectangleClass : BaseClass
   {
      public override string Name => "Rectangle";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messages["x".get()] = (obj, msg) => function<Rectangle>(obj, r => r.X);
         messages["y".get()] = (obj, msg) => function<Rectangle>(obj, r => r.Y);
         messages["width".get()] = (obj, msg) => function<Rectangle>(obj, r => r.Width);
         messages["height".get()] = (obj, msg) => function<Rectangle>(obj, r => r.Height);
         messages["origin".get()] = (obj, msg) => function<Rectangle>(obj, r => r.Origin);
         messages["size".get()] = (obj, msg) => function<Rectangle>(obj, r => r.Size);
         messages["isEmpty".get()] = (obj, msg) => function<Rectangle>(obj, r => r.IsEmpty);
         messages["bottom".get()] = (obj, msg) => function<Rectangle>(obj, r => r.Bottom);
         messages["right".get()] = (obj, msg) => function<Rectangle>(obj, r => r.Right);
         messages["in"] = (obj, msg) => function<Rectangle, IObject>(obj, msg, (r, o) => r.In(o));
         messages["in".Selector("<Point>")] = (obj, msg) => function<Rectangle, Point>(obj, msg, (r, p) => r.In(p));
         messages["in".Selector("<Rectangle>")] = (obj, msg) => function<Rectangle, Rectangle>(obj, msg, (r1, r2) => r1.In(r2));
         messages["notIn"] = (obj, msg) => function<Rectangle, IObject>(obj, msg, (r, o) => r.NotIn(o));
         messages["notIn".Selector("<Point>")] = (obj, msg) => function<Rectangle, Point>(obj, msg, (r, p) => r.NotIn(p));
         messages["notIn".Selector("<Rectangle>")] = (obj, msg) => function<Rectangle, Rectangle>(obj, msg, (r1, r2) => r1.NotIn(r2));
      }
   }
}