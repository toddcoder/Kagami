using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class LazySymbol : Symbol, IHasExpression
{
   protected Expression expression;

   public LazySymbol(Expression expression) => this.expression = expression;

   public override void Generate(OperationsBuilder builder)
   {
      var invokable = new ExpressionInvokable(expression.ToString());
      var _index = builder.RegisterInvokable(invokable, expression, false);
      if (_index)
      {
         builder.PushObject(new Lazy(invokable));
      }
      else
      {
         throw _index.Exception;
      }
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"lazy {expression}";

   public Expression Expression => expression;
}