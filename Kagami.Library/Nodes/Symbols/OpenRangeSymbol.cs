using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class OpenRangeSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.NewOpenRange();

   public override Precedence Precedence => Precedence.Range;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "**";
}