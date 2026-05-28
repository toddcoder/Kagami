using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements;

public class TryBlock(Block block) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      var errorLabel = newLabel("error");

      builder.TryBegin(errorLabel);
      builder.SetErrorHandler(errorLabel);
      block.Generate(builder);
      builder.TryEnd();

      builder.Label(errorLabel);
      builder.NoOp();
   }
}