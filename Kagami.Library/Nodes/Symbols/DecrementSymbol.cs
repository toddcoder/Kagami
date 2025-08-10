using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class DecrementSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.Decrement();

   public override Precedence Precedence => Precedence.AddSubtract;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "---";
}