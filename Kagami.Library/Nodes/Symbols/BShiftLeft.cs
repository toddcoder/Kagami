using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class BShiftLeft : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.BShiftLeft();

   public override Precedence Precedence => Precedence.Shift;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "bsl";
}