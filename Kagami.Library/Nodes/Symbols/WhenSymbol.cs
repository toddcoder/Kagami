using Kagami.Library.Operations;
using Standard.Types.Booleans;

namespace Kagami.Library.Nodes.Symbols
{
   public class WhenSymbol : Symbol
   {
      Expression expression;
      bool reverse;

      public WhenSymbol(Expression expression, bool reverse)
      {
         this.expression = expression;
         this.reverse = reverse;
      }

      public Expression Expression => expression;

      public bool Reverse => reverse;

      public override void Generate(OperationsBuilder builder)
      {
         expression.Generate(builder);
         if (reverse)
            builder.Not();
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => $"when {reverse.Extend("not ")}{expression}";
   }
}