using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class OtherwiseSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.PushBoolean(true);

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => "otherwise";
}