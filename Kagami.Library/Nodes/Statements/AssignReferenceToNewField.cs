using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class AssignReferenceToNewField(string sourceField, string targetField) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.AssignFieldReference(sourceField, targetField);
   }

   public override string ToString() => $"var {sourceField} = ref {targetField}";
}