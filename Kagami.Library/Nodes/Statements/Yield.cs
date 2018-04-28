using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class Yield : Statement
   {
      Expression expression;

      public Yield(Expression expression) => this.expression = expression;

      public override void Generate(OperationsBuilder builder)
      {
         expression.Generate(builder);
         builder.Yield();
      }

      public override string ToString() => $"yield {expression}";
   }
}