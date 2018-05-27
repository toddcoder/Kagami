using Kagami.Library.Objects;
using Kagami.Library.Packages;

namespace Kagami.Drawing
{
   public class Drawing : Package
   {
      public override string ClassName => "Drawing";

      public IObject Point(Arguments arguments)
      {
         var x = arguments[0];
         var y = arguments[1];

         return new Point(x, y);
      }
   }
}