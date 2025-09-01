using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class WithSymbol(TaggedExpression[] taggedExpressions) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.SendMessage("clone()", 0);
      builder.Dup();
      foreach (var (tag, expression) in taggedExpressions)
      {
         builder.Dup();
         expression.Generate(builder);
         builder.SendMessage(tag.set(), 1);
      }
   }

   public override Precedence Precedence => Precedence.PostfixOperator;

   public override Arity Arity => Arity.Postfix;

   public override string ToString() => "with";
}