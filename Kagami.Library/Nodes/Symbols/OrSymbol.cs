using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class OrSymbol : Symbol
   {
      protected Expression expression;

      public OrSymbol(Expression expression) => this.expression = expression;

      public override void Generate(OperationsBuilder builder)
      {
         var label = newLabel("true");
         builder.GoToIfTrue(label);

         expression.Generate(builder);
         builder.GoToIfTrue(label);

         builder.PushBoolean(false);
         builder.Advance(2);

         builder.Label(label);
         builder.PushBoolean(true);

         builder.NoOp();
      }

      public override Precedence Precedence => Precedence.Or;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => $"or ({expression})";
   }
}