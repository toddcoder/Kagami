using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class Put : Statement
   {
      Expression expression;

      public Put(Expression expression) => this.expression = expression;

      public override void Generate(OperationsBuilder builder)
      {
         expression.Generate(builder);

         builder.Put();
      }

      public override string ToString() => $"put {expression}";
   }
}