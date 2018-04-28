using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class InvokableExpressionSymbol : Symbol, IInvokableObject
   {
      Expression expression;

      public InvokableExpressionSymbol(Expression expression)
      {
         this.expression = expression;
         Invokable = new ExpressionInvokable(expression.ToString());
      }

      public override void Generate(OperationsBuilder builder)
      {
         expression.Generate(builder);
         builder.Return(true);
      }

      public IInvokable Invokable { get; set; }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => $"return {expression}";
   }
}