using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class DivModSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.DivRem();

   public override Precedence Precedence => Precedence.MultiplyDivide;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "div.mod";
}