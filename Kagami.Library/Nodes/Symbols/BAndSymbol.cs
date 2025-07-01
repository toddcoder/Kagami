using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class BAndSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.BAnd();

   public override Precedence Precedence => Precedence.BitAnd;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "band";
}