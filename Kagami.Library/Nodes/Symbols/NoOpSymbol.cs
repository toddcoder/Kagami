using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class NoOpSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.NoOp();

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => "<no-op>";
}