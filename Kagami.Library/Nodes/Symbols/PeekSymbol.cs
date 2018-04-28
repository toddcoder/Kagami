using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class PeekSymbol : Symbol
   {
      Expression expression;

      public PeekSymbol(Expression expression) => this.expression = expression;

      public override void Generate(OperationsBuilder builder)
      {
         expression.Generate(builder);
         builder.Dup();
         builder.PrintLine();
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => $"peek({expression})";
   }
}