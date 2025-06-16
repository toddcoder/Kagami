using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class SubexpressionSymbol : Symbol, IHasExpression
{
   protected Expression expression;
   protected bool monoTuple;

   public SubexpressionSymbol(Expression expression, bool monoTuple = false)
   {
      this.expression = expression;
      this.monoTuple = monoTuple;
   }

   public override void Generate(OperationsBuilder builder)
   {
      var endLabel = newLabel("end");
      var tupleLabel = newLabel("tuple");

      expression.Generate(builder);

      builder.IsClass("Sequence", false);
      builder.GoToIfTrue(tupleLabel);

      if (monoTuple)
      {
         builder.NewMonoTuple();
      }

      builder.GoTo(endLabel);

      builder.Label(tupleLabel);
      builder.NewTuple();

      builder.Label(endLabel);
      builder.NoOp();
   }

   public Expression Expression => expression;

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"({expression})";
}