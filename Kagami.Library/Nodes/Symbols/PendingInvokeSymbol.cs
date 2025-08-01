using Kagami.Library.Nodes.Statements;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;
using Return = Kagami.Library.Nodes.Statements.Return;

namespace Kagami.Library.Nodes.Symbols;

public class PendingInvokeSymbol(Symbol invokeSymbol) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var expression = new Expression(invokeSymbol);
      var returnStatement = new Return(expression, nil);
      var block = new Block(returnStatement);
      var lambda = new LambdaSymbol(1, block);
      lambda.Generate(builder);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;
}