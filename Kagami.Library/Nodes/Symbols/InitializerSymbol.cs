using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class InitializerSymbol(IEnumerable<(string property, Expression expression)> properties) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      foreach (var (property, expression) in properties)
      {
         builder.Dup();
         expression.Generate(builder);
         builder.SendMessage(property.set(), 1);
      }
   }

   public override Precedence Precedence => Precedence.PostfixOperator;

   public override Arity Arity => Arity.Postfix;
}