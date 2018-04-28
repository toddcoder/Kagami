using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class Return : Statement
   {
      Expression expression;

      public Return(Expression expression) => this.expression = expression;

      public override void Generate(OperationsBuilder builder)
      {
         expression.Generate(builder);
         builder.Peek(Index);
         builder.Return(true);
      }

      public override string ToString() => $"return {expression}";
   }
}