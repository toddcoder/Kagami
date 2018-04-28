using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class SetSliceSymbol : Symbol
   {
      Expression expression;

      public SetSliceSymbol(Expression expression) => this.expression = expression;

      public override void Generate(OperationsBuilder builder) => builder.SendMessage("=", expression);

      public override Precedence Precedence => Precedence.ChainedOperator;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => $"= {expression}";
   }
}