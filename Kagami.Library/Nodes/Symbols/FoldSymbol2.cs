using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class FoldSymbol2(bool left) : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.Fold(left);

   public override Precedence Precedence => Precedence.ChainedOperator;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => $"fold.{(left ? "left" : "right")}";
}