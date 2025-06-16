using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class MatchAssign : Statement
{
   protected Expression comparisand;
   protected Expression expression;

   public MatchAssign(Expression comparisand, Expression expression)
   {
      this.comparisand = comparisand;
      this.expression = expression;
   }

   public override void Generate(OperationsBuilder builder)
   {
      var fieldName = Guid.NewGuid().ToString();
      builder.NewField(fieldName, false, true);
      expression.Generate(builder);
      builder.Peek(Index);
      builder.AssignField(fieldName, false);
      comparisand.Generate(builder);
      builder.GetField(fieldName);
      builder.Swap();
      builder.Match();
      builder.Drop();
   }

   public override string ToString() => $"when {comparisand} = {expression}";
}