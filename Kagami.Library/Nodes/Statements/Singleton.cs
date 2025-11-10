using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements;

public class Singleton(string identifier, Block block, TypeConstraint typeConstraint) : Statement
{
   protected string functionName = $"__${identifier}_{typeConstraint.AsString}";

   public override void Generate(OperationsBuilder builder)
   {
      var function = new Function(functionName, Parameters.Empty, block, false, false, "", true);
      function.Generate(builder);

      var fieldExistsLabel = newLabel("field-exists");

      builder.FieldExists(identifier);
      builder.GoToIfTrue(fieldExistsLabel);

      builder.Invoke(functionName, 0);
      builder.StoreField(identifier, false, true, typeConstraint);

      builder.Label(fieldExistsLabel);
      builder.GetField(identifier);
   }
}