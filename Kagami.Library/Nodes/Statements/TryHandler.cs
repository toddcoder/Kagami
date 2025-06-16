using Core.Monads;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements;

public class TryHandler(Block block, Block handler, Maybe<string> _errorField, Maybe<Block> _finally) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      var tryLabel = newLabel("try");
      var errorLabel = newLabel("error");
      var endLabel = newLabel("end");

      builder.TryBegin(tryLabel);
      builder.SetErrorHandler(errorLabel);
      block.Generate(builder);
      builder.Label(tryLabel);
      builder.PopTryFrame();
      builder.GoTo(endLabel);

      builder.Label(errorLabel);
      if (_errorField is (true, var errorField))
      {
         builder.PushFrame();
         builder.NewField(errorField, false, true);
         builder.AssignField(errorField, true);
      }
      else
      {
         builder.Drop();
      }

      handler.Generate(builder);

      if (_errorField)
      {
         builder.PopFrame();
      }

      builder.Label(endLabel);
      if (_finally is (true, var finallyBlock))
      {
         finallyBlock.Generate(builder);
      }
      else
      {
         builder.NoOp();
      }
   }
}