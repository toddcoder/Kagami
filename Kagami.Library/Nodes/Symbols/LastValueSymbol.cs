using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class LastValueSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.LastValue();

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => "$";
}