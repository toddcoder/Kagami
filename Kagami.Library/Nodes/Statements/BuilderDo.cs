using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Statements;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements;

public class BuilderDo(BuilderState builderState, Block block) : BuilderStatement(builderState)
{
   public override void Generate(OperationsBuilder builder)
   {
      Prefix(builder);

      var tryLabel = newLabel("try");
      var successLabel = newLabel("success");
      var errorLabel = newLabel("error");
      var endLabel = newLabel("end");

      builder.TryBegin(successLabel);
      builder.SetErrorHandler(errorLabel);
      block.Generate(builder);
      builder.Label(tryLabel);
      builder.PopTryFrame();
      builder.Label(successLabel);
      Assign(builder, KUnit.Value);
      builder.GoTo(endLabel);

      builder.Label(errorLabel);
      AssignFailure(builder);

      builder.Label(endLabel);
      builder.NoOp();
   }

   public override string ToString() => $"exec {{{block}}} [builder]";
}