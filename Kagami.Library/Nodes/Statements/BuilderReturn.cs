using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Statements;

namespace Kagami.Library.Nodes.Statements;

public class BuilderReturn(BuilderState builderState, Expression expression) : BuilderStatement(builderState)
{
   public override void Generate(OperationsBuilder builder)
   {
      Prefix(builder);

      expression.Generate(builder);
      builder.Success();
      Assign(builder);

      builder.Label(builderState.FailureLabel);
      builder.NoOp();
   }

   public override string ToString() => $"return {expression} [builder]";
}