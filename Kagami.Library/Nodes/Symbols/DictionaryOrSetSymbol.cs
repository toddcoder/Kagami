using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class DictionaryOrSetSymbol(Expression expression) : Symbol, IHasExpression
{
   public override void Generate(OperationsBuilder builder)
   {
      expression.Generate(builder);
      builder.NewDictionaryOrSet();
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public Expression Expression => expression;

   public override string ToString() => $"{{{expression}}}";
}