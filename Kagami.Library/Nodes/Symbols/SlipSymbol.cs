using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class SlipSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.NewSlip();

   public override Precedence Precedence => Precedence.PrefixOperator;

   public override Arity Arity => Arity.Prefix;
}