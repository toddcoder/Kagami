using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class RaiseSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.Raise();

   public override Precedence Precedence => Precedence.Raise;

   public override Arity Arity => Arity.Binary;

   public override bool LeftToRight => false;

   public override string ToString() => "^";
}