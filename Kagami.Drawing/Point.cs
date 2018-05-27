using Kagami.Library.Objects;
using Standard.Types.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Drawing
{
   public struct Point : IObject
   {
      IObject x;
      IObject y;

      public Point(IObject x, IObject y) : this()
      {
         this.x = x;
         this.y = y;
      }

      public string ClassName => "Point";

      public string AsString => $"{x.AsString} {y.AsString}";

      public string Image => $"Point({x.Image}, {y.Image})";

      public int Hash => (x.Hash + y.Hash).GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Point point && x.IsEqualTo(point.x) && y.IsEqualTo(point.y);

      public bool IsTrue => x.IsTrue && y.IsTrue;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public IObject X => x;

      public IObject Y => y;
   }
}