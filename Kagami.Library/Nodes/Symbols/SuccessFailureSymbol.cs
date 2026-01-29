using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class SuccessFailureSymbol(Expression successExpression, Expression failureExpression) :Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var failureLabel = newLabel("failure");
      var endLabel = newLabel("end");

      builder.GoToIfFalse(failureLabel);

      successExpression.Generate(builder);
      builder.Success();
      builder.GoTo(endLabel);

      builder.Label(failureLabel);
      failureExpression.Generate(builder);
      builder.Failure();

      builder.Label(endLabel);
      builder.NoOp();
   }

   public override Precedence Precedence => Precedence.Pipeline;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => $"!! {successExpression} : {failureExpression}";
}