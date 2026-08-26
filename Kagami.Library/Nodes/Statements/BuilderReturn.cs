using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Statements;

namespace Kagami.Library.Nodes.Statements;

public class BuilderReturn(BuilderState state, Expression expression) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.GetField(state.ResultFieldName);
      builder.GoToIfFalse(state.FailureLabel, false);

      expression.Generate(builder);
      builder.Success();
      builder.AssignField(state.ResultFieldName, false);

      builder.Label(state.FailureLabel);
      builder.NoOp();
   }

   public override string ToString() => $"return {expression} [builder]";
}