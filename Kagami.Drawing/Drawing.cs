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
         module.RegisterClass(new SizeClass());
         module.RegisterClass(new RectangleClass());
      }

      public Point Point(IObject x, IObject y) => new(x, y);

      public Point Point(Size size) => new(size.Width, size.Height);

      public Size Size(IObject width, IObject height) => new(width, height);

      public Size Size(Point point) => new(point.X, point.Y);

      public Rectangle Rectangle(IObject x, IObject y, IObject width, IObject height) => new(x, y, width, height);

      public Rectangle Rectangle(Point origin, Size size) => new(origin, size);
   }
}