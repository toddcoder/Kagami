using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class ForwardReductionSymbol(string operatorSource, Expression expression, bool cumulative) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      expression.Generate(builder);
      builder.PushString(operatorSource);
      builder.Join(cumulative);
      builder.ExecuteString();
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;
}