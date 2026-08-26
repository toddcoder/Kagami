using Kagami.Library.Operations;
using Kagami.Library.Parsers.Statements;

namespace Kagami.Library.Nodes.Statements;

public class BuilderDo(BuilderState state, Block block) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.GetField(state.ResultFieldName);
      builder.GoToIfFalse(state.FailureLabel, false);

      block.Generate(builder);
   }

   public override string ToString() => $"do {{{block}}} [builder]";
}