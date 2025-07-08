using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class AssertSymbol : Symbol
{
   protected Expression condition;
   protected Expression value;
   protected Expression error;

   public AssertSymbol(Expression condition, Expression value, Expression error)
   {
      this.condition = condition;
      this.value = value;
      this.error = error;
   }

   public override void Generate(OperationsBuilder builder)
   {
      var trueLabel = newLabel("true");
      var endLabel = newLabel("end");

      condition.Generate(builder);
      builder.GoToIfTrue(trueLabel);

      error.Generate(builder);
      builder.Failure();

      builder.GoTo(endLabel);

      builder.Label(trueLabel);
      value.Generate(builder);
      builder.Success();

      builder.Label(endLabel);
      builder.NoOp();
   }

   public override Precedence Precedence => Precedence.ChainedOperator;

   public override Arity Arity => Arity.Nullary;

   public override string ToString()
   {
      return $"assert {condition} then {value} else {error}";
   }
}