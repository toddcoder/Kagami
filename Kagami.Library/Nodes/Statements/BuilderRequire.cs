using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Statements;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements;

public class BuilderRequire(BuilderState builderState, Expression assertion, Expression ifFalse) : BuilderStatement(builderState), IHasExpressions
{
   public override void Generate(OperationsBuilder builder)
   {
      var failureLabel = newLabel("failure");
      var endLabel = newLabel("end");

      Prefix(builder);

      assertion.Generate(builder);
      builder.GoToIfFalse(failureLabel);

      Assign(builder, KUnit.Value);
      builder.GoTo(endLabel);

      builder.Label(failureLabel);
      ifFalse.Generate(builder);
      Assign(builder);

      builder.Label(endLabel);
      builder.NoOp();
   }

   public Expression[] Expressions => [assertion, ifFalse];
}