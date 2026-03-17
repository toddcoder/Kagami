using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class ForwardReductionSymbol(string operatorSource, Expression expression) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      expression.Generate(builder);
      builder.PushString(operatorSource);
      builder.Join();
      builder.CreateLambda();
      builder.SendMessage("invoke()", 0);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;
}