using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class AndSymbol : Symbol, IHasExpression
{
   protected Expression expression;

   public AndSymbol(Expression expression)
   {
      this.expression = expression;
   }

   public override void Generate(OperationsBuilder builder)
   {
      var intLabel = newLabel("int");
      var falseLabel = newLabel("false");
      var endLabel = newLabel("end");

      builder.IsClass("Int", false);
      builder.GoToIfTrue(intLabel);

      builder.IsClass("Char", false);
      builder.GoToIfTrue(intLabel);

      builder.IsClass("String", false);
      builder.GoToIfTrue(intLabel);

      builder.GoToIfFalse(falseLabel);

      expression.Generate(builder);
      builder.GoToIfFalse(falseLabel);

      builder.PushBoolean(true);
      builder.Advance(2);

      builder.Label(falseLabel);
      builder.PushBoolean(false);
      builder.GoTo(endLabel);

      builder.Label(intLabel);
      expression.Generate(builder);
      builder.And();

      builder.Label(endLabel);
      builder.NoOp();
   }

   public override Precedence Precedence => Precedence.And;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => $"and ({expression})";

   public Expression Expression => expression;
}