using Kagami.Library.Objects;
using Core.Collections;
using static Kagami.Drawing.DrawingFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Drawing;

public readonly struct Point : IObject
{
   private readonly IObject x;
   private readonly IObject y;

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

   public Guid Id { get; init; } = Guid.NewGuid();

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public IObject X => x;

   public IObject Y => y;

   public Point Offset(IObject x, IObject y)
   {
      var newX = add(this.x, x);
      var newY = add(this.y, y);

      return new Point(newX, newY);
   }

   public KBoolean IsEmpty => isZero(x) && isZero(y);
}