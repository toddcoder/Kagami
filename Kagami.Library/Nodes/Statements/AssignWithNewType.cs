using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class AssignWithNewType(string fieldName, string className, Expression expression) : Statement, IHasExpression
{
   public override void Generate(OperationsBuilder builder)
   {
      expression.Generate(builder);
      builder.AssignFieldWithType(fieldName, className);
   }

   public Expression Expression => expression;

   public override string ToString() => $"{fieldName} {className} = {expression}";
}