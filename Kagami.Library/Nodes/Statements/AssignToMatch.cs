using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class AssignToMatch : Statement
   {
      protected Symbol comparisand;
      protected Expression expression;

      public AssignToMatch(Symbol comparisand, Expression expression)
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

      public override string ToString() => $"set {comparisand} = {expression}";
   }
}