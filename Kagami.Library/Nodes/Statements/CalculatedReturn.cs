using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class CalculatedReturn(TaggedExpression[] taggedExpressions, Expression expression) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.PushFrame();

      foreach (var (tag, expr) in taggedExpressions)
      {
         builder.NewField(tag, false, true);
         expr.Generate(builder);
         builder.AssignField(tag, false);
         builder.Drop();
      }

      expression.Generate(builder);
      builder.PopFrameWithValue();

      builder.Return(true);
   }
}