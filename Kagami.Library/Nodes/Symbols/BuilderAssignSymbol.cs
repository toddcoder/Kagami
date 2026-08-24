using Kagami.Library.Invokables;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class BuilderAssignSymbol(string fieldName, Expression expression, string failureLabel) : Symbol, IHasExpression
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.Dup();
      builder.Dup();
      builder.SetRegister(1);

      var lambdaSymbol = new LambdaSymbol(Parameters.Empty, expression);
      lambdaSymbol.Generate(builder);

      builder.SendMessage("assign(_<Lambda>)", 1);
      builder.Dup();
      builder.Dup();
      builder.SetRegister(0);
      builder.GoToIfFalse(failureLabel);
      builder.StoreField(fieldName, false, false, false, nil);
   }

   public override Precedence Precedence => Precedence.ChainedOperator;

   public override Arity Arity => Arity.Binary;

   public Expression Expression => expression;
}