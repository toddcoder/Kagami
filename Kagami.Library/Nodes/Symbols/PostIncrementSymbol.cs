using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class PostIncrementSymbol(bool increment) : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.PostIncrement(increment);

   public override Precedence Precedence => Precedence.PostfixOperator;

   public override Arity Arity => Arity.Postfix;
}