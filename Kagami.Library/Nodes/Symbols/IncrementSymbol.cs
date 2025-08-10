using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class IncrementSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.Increment();

   public override Precedence Precedence => Precedence.AddSubtract;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "+++";
}