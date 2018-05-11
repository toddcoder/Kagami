using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class Return : Statement
   {
      Expression expression;

      public Return(Expression expression) => this.expression = expression;

      public override void Generate(OperationsBuilder builder) => builder.Return(expression, this);

      public override string ToString() => $"return {expression}";
   }
}