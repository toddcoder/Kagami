using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements;

public class AssignOperator(string fieldName, Expression expression) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var assignLabel = newLabel("assign");

      builder.FieldExists(fieldName);
      builder.GoToIfTrue(assignLabel);

      builder.NewField(fieldName, true, true);

      builder.Label(assignLabel);
      expression.Generate(builder);
      builder.AssignField(fieldName, true);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"{fieldName} := {expression}";
}