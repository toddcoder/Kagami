using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class RangeSymbol : Symbol
{
   protected bool inclusive;
   protected bool down;

   public RangeSymbol(bool inclusive, bool down)
   {
      this.inclusive = inclusive;
      this.down = down;
   }

   public override void Generate(OperationsBuilder builder) => builder.NewRange(inclusive, down);

   public override Precedence Precedence => Precedence.Range;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => inclusive ? "..." : "..<";
}