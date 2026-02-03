using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class DslInvokeSymbol(string className, Expression[] expressions) : Symbol, IHasExpressions
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.Invoke(className, 0);
      foreach (var expression in expressions)
      {
         expression.Generate(builder);
         builder.SendMessage("append(_)", 1);
      }
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public Expression[] Expressions => expressions;
}