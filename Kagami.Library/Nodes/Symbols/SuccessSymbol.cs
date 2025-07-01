using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class SuccessSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.Success();

   public override Precedence Precedence => Precedence.PostfixOperator;

   public override Arity Arity => Arity.Postfix;

   public override string ToString() => "!";
}