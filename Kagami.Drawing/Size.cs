using Kagami.Library.Objects;
using Core.Collections;
using static Kagami.Drawing.DrawingFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Drawing
{
   public class Size : IObject
   {
      IObject width;
      IObject height;

      public Size(IObject width, IObject height)
      {
         this.width = width;
         this.height = height;
      }

      public string ClassName => "Size";

      public string AsString => $"{width.AsString} {height.AsString}";

      public string Image => $"Size({width.Image}, {height.Image})";

      public int Hash => (width.Hash + height.Hash).GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Size size && width.IsEqualTo(size.width) && height.IsEqualTo(size.height);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => width.IsTrue && height.IsTrue;

      public IObject Width => width;

      public IObject Height => height;

      public IObject Add(Size size)
      {
         var newWidth = add(width, size.width);
         var newHeight = add(height, size.height);

         return new Size(newWidth, newHeight);
      }

      public IObject Subtract(Size size)
      {
         var newWidth = subtract(width, size.width);
         var newHeight = subtract(height, size.height);

         return new Size(newWidth, newHeight);
      }

      public Boolean IsEmpty => isZero(width) && isZero(height);
   }
}