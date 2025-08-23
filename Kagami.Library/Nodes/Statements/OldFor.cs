using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;
using Expression = Kagami.Library.Nodes.Symbols.Expression;

namespace Kagami.Library.Nodes.Statements;

public class OldFor(string identifier, Expression initializer, Expression condition, Block block, Statement increment) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      var beginLabel = newLabel("begin");
      var endLabel = newLabel("end");
      var exitLabel = newLabel("exit");
      var skipLabel = newLabel("skip");

      builder.PushFrame();

      builder.NewField(identifier, true, true);
      initializer.Generate(builder);
      builder.AssignField(identifier, false);

      builder.Label(beginLabel);
      builder.PushFrame();

      condition.Generate(builder);

      builder.GoToIfFalse(endLabel);

      builder.PushExitFrame(exitLabel);
      builder.PushSkipFrame(skipLabel);
      block.Generate(builder);
      increment.Generate(builder);
      builder.PopFrame();
      builder.Label(skipLabel);
      builder.PopFrame();
      builder.PopFrame();
      builder.GoTo(beginLabel);

      builder.Label(endLabel);
      builder.PopFrame();

      builder.Label(exitLabel);
      builder.PopFrame();
   }
}