using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class PreIncrementSymbol(bool increment) : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.PreIncrement(increment);

   public override Precedence Precedence => Precedence.PrefixOperator;

   public override Arity Arity => Arity.Prefix;

   public override string ToString() => increment ? "++" : "--";
}