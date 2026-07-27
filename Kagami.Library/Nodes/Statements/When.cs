using Core.Strings;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements;

public class When((Expression, Block)[] expressionBlock, string fieldName, bool mutable) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.PushFrame();

      var beginLabel = newLabel("begin");
      var endLabel = newLabel("end");

      foreach (var (expression, block) in expressionBlock)
      {
         builder.Label(beginLabel);
         expression.Generate(builder);
         var nextLabel = newLabel("begin");
         builder.GoToIfFalse(nextLabel);
         beginLabel = nextLabel;
         block.Generate(builder);
         builder.GoTo(endLabel);
      }

      builder.Label(beginLabel);
      builder.PushString("No true condition found");
      builder.Throw();

      builder.Label(endLabel);

      if (fieldName.IsNotEmpty())
      {
         builder.PopFrameWithValue();
         builder.StoreField(fieldName, mutable, true, false, nil);
      }
      else
      {
         builder.PopFrame();
      }
   }
}