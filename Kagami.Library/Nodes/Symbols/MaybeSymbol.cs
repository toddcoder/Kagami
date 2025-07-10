using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class MaybeSymbol(Expression booleanExpression, Expression expression) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var noneLabel = newLabel("none");
      var endLabel = newLabel("end");
      booleanExpression.Generate(builder);
      builder.GoToIfFalse(noneLabel);

      expression.Generate(builder);
      builder.Some();
      builder.GoTo(endLabel);

      builder.Label(noneLabel);
      builder.PushNil();

      builder.Label(endLabel);
      builder.NoOp();
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"maybe {booleanExpression} then {expression}";
}