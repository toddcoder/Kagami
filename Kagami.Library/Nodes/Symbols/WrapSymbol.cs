using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class WrapSymbol(string protocolName) : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.ProtocolWrap(protocolName);

   public override Precedence Precedence => Precedence.PrefixOperator;

   public override Arity Arity => Arity.Prefix;

   public override string ToString() => $"//{protocolName}";
}