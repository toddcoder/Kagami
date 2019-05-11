using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements
{
   public class For2 : Statement
   {
      Symbol comparisand;
      Expression source;
      Block block;

      public For2(Symbol comparisand, Expression source, Block block)
      {
         this.comparisand = comparisand;
         this.source = source;
         this.block = block;
      }

      public override void Generate(OperationsBuilder builder)
      {
         var topLabel = newLabel("top");
         var endLabel = newLabel("end");
         var exitLabel = newLabel("exit");
         var skipLabel = newLabel("skip");
         var backToTopLabel = newLabel("back.to.top");

         builder.PushExitFrame(exitLabel);
         var iteratorName = newLabel("iterator");
         builder.NewField(iteratorName, false, true);
         source.Generate(builder);
         builder.Peek(Index);
         builder.GetIterator(false);
         builder.AssignField(iteratorName, false);

         builder.Label(topLabel);
         builder.PushFrame();
         comparisand.Generate(builder);
         builder.GetField(iteratorName);
         builder.SendMessage("next()", 0);
         builder.GoToIfNone(endLabel);
         builder.Swap();
         builder.Match();
         builder.GoToIfTrue(backToTopLabel);

         builder.PopFrame();
         builder.GoTo(topLabel);

         builder.Label(backToTopLabel);
         builder.PushSkipFrame(skipLabel);
         block.Generate(builder);
         builder.PopFrame();
         builder.Label(skipLabel);
         builder.PopFrame();
         builder.GoTo(topLabel);

         builder.Label(endLabel);
         builder.PopFrame();
         builder.PopFrame();

         builder.Label(exitLabel);
         builder.NoOp();
      }

      public override string ToString() => $"for {comparisand} <- {source} {block}";
   }
}