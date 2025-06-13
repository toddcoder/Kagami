using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class NoneSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.PushNone();

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => "none";
}