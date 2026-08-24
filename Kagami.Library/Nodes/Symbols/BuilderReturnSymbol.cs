using Kagami.Library.Invokables;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class BuilderReturnSymbol(Expression expression, string failureLabel) : Symbol, IHasExpression
{
   public override void Generate(OperationsBuilder builder)
   {
      var endLabel = newLabel("end");

      builder.GetRegister(1);

      var lambdaSymbol = new LambdaSymbol(Parameters.Empty, expression);
      lambdaSymbol.Generate(builder);

      builder.SendMessage("return(_<Lambda>)", 1);
      builder.GoTo(endLabel);

      builder.Label(failureLabel);
      builder.GetRegister(0);

      builder.Label(endLabel);
      builder.NoOp();
   }

   public override Precedence Precedence => Precedence.ChainedOperator;

   public override Arity Arity => Arity.Binary;

   public Expression Expression => expression;
}