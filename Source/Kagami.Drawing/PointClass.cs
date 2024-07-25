using Kagami.Library;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Drawing
{
   public class PointClass : BaseClass
   {
      public override string Name => "Point";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messages["x".get()] = (obj, _) => function<Point>(obj, p => p.X);
         messages["y".get()] = (obj, _) => function<Point>(obj, p => p.Y);
         messages["offset"] = (obj, msg) => function<Point, IObject, IObject>(obj, msg, (p, x, y) => p.Offset(x, y));
         messages["isEmpty".get()] = (obj, _) => function<Point>(obj, p => p.IsEmpty);
      }
   }
}