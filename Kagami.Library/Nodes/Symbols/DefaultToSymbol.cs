using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class DefaultToSymbol(Expression expression) : Symbol, IHasExpression
{
   public override void Generate(OperationsBuilder builder)
   {
      var labelEvaluateOptional = newLabel("eval-optional");
      var labelEvaluateResult = newLabel("eval-result");
      var labelEnd = newLabel("end");

      builder.Dup();
      builder.IsOptional();
      builder.GoToIfTrue(labelEvaluateOptional);

      builder.Dup();
      builder.IsResult();
      builder.GoToIfTrue(labelEvaluateResult);

      builder.GoTo(labelEnd);

      builder.Label(labelEvaluateOptional);
      builder.GoToIfSome(labelEnd);

      expression.Generate(builder);
      builder.GoTo(labelEnd);

      builder.Label(labelEvaluateResult);
      builder.GoToIfSuccess(labelEnd);

      expression.Generate(builder);

      builder.Label(labelEnd);
      builder.NoOp();
   }

   public override Precedence Precedence => Precedence.ChainedOperator;

   public override Arity Arity => Arity.Binary;

   public Expression Expression => expression;

   public override string ToString() => $"?: {expression}";
}