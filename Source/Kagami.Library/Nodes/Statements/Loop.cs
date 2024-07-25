using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements
{
   public class Loop : Statement
   {
      protected Block block;
      protected Expression expression;

      public Loop(Block block, Expression expression)
      {
         this.block = block;
         this.expression = expression;
      }

      public override void Generate(OperationsBuilder builder)
      {
         var beginLabel = newLabel("begin");
         var exitLabel = newLabel("exit");
         var skipLabel = newLabel("skip");
         var untilLabel = newLabel("until");
         var endLabel = newLabel("end");

         builder.Label(beginLabel);

         builder.PushFrame();
         builder.PushExitFrame(exitLabel);
         builder.PushSkipFrame(skipLabel);

         block.Generate(builder);

         builder.PopFrame();
         builder.Label(skipLabel);
         builder.PopFrame();

         expression.Generate(builder);
         builder.GoToIfFalse(untilLabel);

         builder.PopFrame();

         builder.Label(exitLabel);
         builder.GoTo(endLabel);

         builder.Label(untilLabel);
         builder.PopFrame();
         builder.GoTo(beginLabel);

         builder.Label(endLabel);
         builder.NoOp();
      }
   }
}