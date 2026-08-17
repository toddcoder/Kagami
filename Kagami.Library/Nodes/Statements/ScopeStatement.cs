using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Statements;

public class ScopeStatement(string fieldName, Expression expression) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.PushFrame();
      var assignToNewField = new AssignToNewField(false, fieldName, expression, nil, false, false);
      assignToNewField.Generate(builder);
   }
}