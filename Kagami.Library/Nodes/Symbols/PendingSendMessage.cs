using Kagami.Library.Nodes.Statements;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using Return = Kagami.Library.Nodes.Statements.Return;

namespace Kagami.Library.Nodes.Symbols;

public class PendingSendMessage(SendMessageSymbol sendMessageSymbol) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var innerBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
      innerBuilder.Add(new FieldSymbol("__$0"));
      innerBuilder.Add(sendMessageSymbol);
      var expression = innerBuilder.ToExpression().ForceValue();
      var returnStatement = new Return(expression, nil);
      var block = new Block(returnStatement);
      var lambda = new LambdaSymbol(1, block);
      lambda.Generate(builder);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"_{sendMessageSymbol}";
}