using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class BShiftRight : Symbol
   {
      public override void Generate(OperationsBuilder builder) => builder.BShiftRight();

      public override Precedence Precedence => Precedence.Shift;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => "bsr";
   }
}