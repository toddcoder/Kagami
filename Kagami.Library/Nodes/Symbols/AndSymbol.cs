using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class AndSymbol : Symbol
   {
      Expression expression;

      public AndSymbol(Expression expression) => this.expression = expression;

      public override void Generate(OperationsBuilder builder)
      {
         var label = newLabel("false");
         builder.GoToIfFalse(label);

         expression.Generate(builder);
         builder.GoToIfFalse(label);

         builder.PushBoolean(true);
         builder.Advance(2);

         builder.Label(label);
         builder.PushBoolean(false);

         builder.NoOp();
      }

      public override Precedence Precedence => Precedence.And;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => $"and ({expression})";
   }
}