using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class ImageSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.Image();

   public override Precedence Precedence => Precedence.PrefixOperator;

   public override Arity Arity => Arity.Prefix;

   public override string ToString() => "~";
}