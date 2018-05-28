using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;

namespace Kagami.Drawing
{
   public class Drawing : Package
   {
      public override string ClassName => "Drawing";

      public override void LoadTypes(Module module)
      {
         module.RegisterClass(new DrawingClass());
         module.RegisterClass(new PointClass());
      }

      public IObject Point(Arguments arguments)
      {
         var x = arguments[0];
         var y = arguments[1];

         return new Point(x, y);
      }
   }
}