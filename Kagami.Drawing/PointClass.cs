using Kagami.Library;
using Kagami.Library.Classes;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Drawing
{
   public class PointClass : BaseClass
   {
      public override string Name => "Point";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messages["x".get()] = (obj, msg) => function<Point>(obj, p => p.X);
         messages["y".get()] = (obj, msg) => function<Point>(obj, p => p.Y);
      }
   }
}