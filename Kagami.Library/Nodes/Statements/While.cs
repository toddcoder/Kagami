using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements;

public class While : Statement
{
   protected Expression expression;
   protected Block block;
   protected bool isWhile;

   public While(Expression expression, Block block, bool isWhile)
   {
      this.expression = expression;
      this.block = block;
      this.isWhile = isWhile;
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
      builder.Peek(Index);

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
      builder.NoOp();
   }

   public override string ToString() => $"{(isWhile ? "while" : "until")} {expression} {block}";
}