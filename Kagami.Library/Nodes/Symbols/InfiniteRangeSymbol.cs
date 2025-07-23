using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class InfiniteRangeSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.PushInt(1);
      builder.NewOpenRange();
   }

   public override Precedence Precedence => Precedence.PostfixOperator;

   public override Arity Arity => Arity.Postfix;

   public override string ToString() => "..*";
}