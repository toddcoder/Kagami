using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class AssignToNewField2 : Statement
   {
      protected Expression comparisand;
      protected Expression expression;

      public AssignToNewField2(Expression comparisand, Expression expression)
      {
         this.comparisand = comparisand;
         this.expression = expression;
      }

      public override void Generate(OperationsBuilder builder)
      {
         expression.Generate(builder);
         builder.Peek(Index);
         comparisand.Generate(builder);
         builder.Match();
         builder.Drop();
      }

      public override string ToString() => $"| {comparisand} = {expression}";

      public void Deconstruct(out Expression comparisand, out Expression expression)
      {
         comparisand = this.comparisand;
         expression = this.expression;
      }
   }
}