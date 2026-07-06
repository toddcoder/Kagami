using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class BindComparisandSymbol(string name, Expression expression) : Symbol, IHasExpression
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.PushString(name);
      expression.Generate(builder);
      builder.BindComparisand();
   }

   public override Precedence Precedence => Precedence.KeyValue;

   public override Arity Arity => Arity.Binary;

   public Expression Expression => expression;
}