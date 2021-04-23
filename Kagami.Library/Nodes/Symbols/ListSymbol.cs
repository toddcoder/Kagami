using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class ListSymbol : Symbol
   {
      protected Expression expression;

      public ListSymbol(Expression expression) => this.expression = expression;

      public override void Generate(OperationsBuilder builder)
      {
         expression.Generate(builder);
         builder.NewList();
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => $"[{expression}]";
   }
}