using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class LazySymbol : Symbol
   {
      Expression expression;

      public LazySymbol(Expression expression) => this.expression = expression;

      public override void Generate(OperationsBuilder builder)
      {
         var invokable = new ExpressionInvokable(expression.ToString());
         if (builder.RegisterInvokable(invokable, expression, false).If(out _, out var exception))
            builder.PushObject(new Lazy(invokable));
         else
            throw exception;
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => $"lazy {expression}";
   }
}