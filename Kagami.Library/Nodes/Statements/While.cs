using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements;

public class While : Statement
{
   protected Expression expression;
   protected Block block;
   protected bool isWhile;
   protected Maybe<Block> _exitBlock;

   public While(Expression expression, Block block, bool isWhile, Maybe<Block> _exitBlock)
   {
      this.expression = expression;
      this.block = block;
      this.isWhile = isWhile;
      this._exitBlock = _exitBlock;
   }

   public override void Generate(OperationsBuilder builder)
   {
      var beginLabel = newLabel("begin");
      var endLabel = newLabel("end");
      var exitLabel = newLabel("exit");
      var skipLabel = newLabel("skip");

      builder.Label(beginLabel);
      builder.PushFrame();

      expression.Generate(builder);

      if (isWhile)
      {
         builder.GoToIfFalse(endLabel);
      }
      else
      {
         builder.GoToIfTrue(endLabel);
      }

      builder.PushExitFrame(exitLabel);
      builder.PushSkipFrame(skipLabel);
      block.Generate(builder);
      builder.PopFrame();
      builder.Label(skipLabel);
      builder.PopFrame();
      builder.PopFrame();
      builder.GoTo(beginLabel);

      builder.Label(endLabel);
      builder.PopFrame();

      builder.Label(exitLabel);
      if (_exitBlock is (true, var exitBlock))
      {
         exitBlock.Generate(builder);
      }

      builder.NoOp();
   }

   public override string ToString() => $"{(isWhile ? "while" : "until")} {expression} {block}";

   public void AddIncrementerToBlock(Statement statement) => block.Add(statement);
}