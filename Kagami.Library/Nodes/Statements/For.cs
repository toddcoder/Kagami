using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Kagami.Library.Parsers;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements
{
   public class For : Statement
   {
      protected Symbol comparisand;
      protected Expression source;
      protected Block block;
      protected PossibleIfExpression possibleIfExpression;
      protected Maybe<Block> _elseBlock;
      protected Maybe<Block> _exitBlock;

      public For(Symbol comparisand, Expression source, Block block, PossibleIfExpression possibleIfExpression, Maybe<Block> _elseBlock,
         Maybe<Block> _exitBlock)
      {
         this.comparisand = comparisand;
         this.source = source;
         this.block = block;
         this.possibleIfExpression = possibleIfExpression;
         this._elseBlock = _elseBlock;
         this._exitBlock = _exitBlock;
      }

      public For(Symbol comparisand, Expression source, Block block, PossibleIfExpression possibleIfExpression) : this(comparisand, source, block,
         possibleIfExpression, nil, nil)
      {
      }

      public override void Generate(OperationsBuilder builder)
      {
         var topLabel = newLabel("top");
         var endLabel = newLabel("end");
         var exitLabel = newLabel("exit");
         var skipLabel = newLabel("skip");
         var backToTopLabel = newLabel("back.to.top");
         var failedIfLabel = newLabel("failed-if");
         var skippedLabel = newLabel("skipped");
         var finalExitLabel = newLabel("final.exit");

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
         builder.NewField("`i", false, true);
         builder.AssignField("`i", false);
         builder.Swap();
         builder.Match();
         builder.GoToIfTrue(backToTopLabel);

         if (_elseBlock is (true, var elseBlock))
         {
            elseBlock.Generate(builder);
         }

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

         if (_elseBlock)
         {
            builder.GoTo(skippedLabel);
         }

         builder.Label(failedIfLabel);

         if (_elseBlock is (true, var elseBlock2))
         {
            elseBlock2.Generate(builder);
         }

         builder.Label(skippedLabel);
         builder.PopFrame();
         builder.Label(skipLabel);
         builder.PopFrame();
         builder.GoTo(topLabel);

         builder.Label(endLabel);
         builder.PopFrame();
         builder.PopFrame();
         builder.GoTo(finalExitLabel);

         builder.Label(exitLabel);
         if (_exitBlock is (true, var exitedBlock))
         {
            exitedBlock.Generate(builder);
         }

         builder.Label(finalExitLabel);
         builder.NoOp();
      }

      public override string ToString() => $"for {comparisand} in {source} {block}";
   }
}