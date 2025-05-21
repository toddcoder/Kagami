using Kagami.Library.Objects;
using Core.Collections;
using static Kagami.Drawing.DrawingFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Drawing;

public class Rectangle : IObject
{
   protected Point origin;
   protected Size size;

   public Rectangle(IObject x, IObject y, IObject width, IObject height)
   {
      origin = new Point(x, y);
      size = new Size(width, height);
   }

   public Rectangle(Point origin, Size size)
   {
      this.origin = origin;
      this.size = size;
   }

   public string ClassName => "Rectangle";

   public string AsString => $"{origin.AsString} {size.AsString}";

   public string Image => $"Rectangle({origin.X.Image}, {origin.Y.Image}, {size.Width.Image}, {size.Height.Image})";

   public int Hash => (origin.Hash + size.Hash).GetHashCode();

   public bool IsEqualTo(IObject obj)
   {
      return obj is Rectangle r && origin.IsEqualTo(r.origin) && size.IsEqualTo(r.size);
   }

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => origin.IsTrue && size.IsTrue;

   public IObject X => origin.X;

   public IObject Y => origin.Y;

   public IObject Width => size.Width;

   public IObject Height => size.Height;

   public Point Origin => origin;

   public Size Size => size;

   public KBoolean IsEmpty => origin.IsEmpty.IsTrue && size.IsEmpty.IsTrue;

   public IObject Bottom => add(Y, Height);

   public IObject Right => add(X, Width);

   public KBoolean In(IObject obj) => obj switch
   {
      Point point => In(point),
      Rectangle rectangle => In(rectangle),
      _ => false
   };

   public KBoolean NotIn(IObject obj) => !In(obj).IsTrue;

   public KBoolean In(Point point)
   {
      return compare(point.X, X) >= 0 && compare(point.X, Right) <= 0 && compare(point.Y, Y) >= 0 && compare(point.Y, Bottom) >= 0;
   }

   public KBoolean NotIn(Point point) => !In(point).IsTrue;

   public KBoolean In(Rectangle rectangle)
   {
      return In(rectangle.origin).IsTrue && compare(rectangle.Width, Width) <= 0 && compare(rectangle.Height, Height) <= 0;
   }

   public KBoolean NotIn(Rectangle rectangle) => !In(rectangle).IsTrue;
}