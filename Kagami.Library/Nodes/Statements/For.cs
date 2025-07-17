using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Kagami.Library.Parsers;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements
{
   public class For : Statement
   {
      protected Symbol comparisand;
      protected Expression source;
      protected Block block;
      protected PossibleIfExpression possibleIfExpression;

      public For(Symbol comparisand, Expression source, Block block, PossibleIfExpression possibleIfExpression)
      {
         this.comparisand = comparisand;
         this.source = source;
         this.block = block;
         this.possibleIfExpression = possibleIfExpression;
      }

      public override void Generate(OperationsBuilder builder)
      {
         var topLabel = newLabel("top");
         var endLabel = newLabel("end");
         var exitLabel = newLabel("exit");
         var skipLabel = newLabel("skip");
         var backToTopLabel = newLabel("back.to.top");
         var failedIfLabel = newLabel("failed-if");

         builder.PushExitFrame(exitLabel);
         var iteratorName = newLabel("iterator");
         builder.NewField(iteratorName, false, true);
         source.Generate(builder);
         builder.GetIterator(false);
         builder.AssignField(iteratorName, false);

         builder.Label(topLabel);
         builder.PushFrame();
         comparisand.Generate(builder);
         builder.GetField(iteratorName);
         builder.SendMessage("next()", 0);
         builder.GoToIfNil(endLabel);
         builder.Swap();
         builder.Match();
         builder.GoToIfTrue(backToTopLabel);

         builder.PopFrame();
         builder.GoTo(topLabel);

         builder.Label(backToTopLabel);
         builder.PushSkipFrame(skipLabel);

         switch (possibleIfExpression)
         {
            case PossibleIfExpression.If @if:
            {
               @if.Expression.Generate(builder);
               builder.GoToIfFalse(failedIfLabel);
               break;
            }
            case PossibleIfExpression.IfNot ifNot:
            {
               ifNot.Expression.Generate(builder);
               builder.GoToIfTrue(failedIfLabel);
               break;
            }
         }

         block.Generate(builder);

         builder.Label(failedIfLabel);

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

      public override string ToString() => $"for {comparisand} in {source} {block}";
   }
}