using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements;

public class While : Statement
{
   protected Expression expression;
   protected Block block;

   public While(Expression expression, Block block)
   {
      this.expression = expression;
      this.block = block;
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

      builder.GoToIfFalse(endLabel);

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

   public override string ToString() => $"while {expression} {block}";
}