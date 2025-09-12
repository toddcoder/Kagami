using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class ImplicitMapSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.NoOp();

   public override Precedence Precedence => Precedence.SendMessage;

   public override Arity Arity => Arity.Postfix;

   public override string ToString() => "[]";
}