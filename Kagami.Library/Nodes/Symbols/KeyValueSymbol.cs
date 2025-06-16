using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class KeyValueSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.NewKeyValue();

   public override Precedence Precedence => Precedence.KeyValue;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "=>";
}