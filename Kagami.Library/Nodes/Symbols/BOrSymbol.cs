using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class BOrSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.BOr();

   public override Precedence Precedence => Precedence.BitOr;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "bor";
}