using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class XOrSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.XOr();

   public override Precedence Precedence => Precedence.Or;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "xor";
}