using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class SubexpressionSymbol : Symbol
   {
      Expression expression;

      public SubexpressionSymbol(Expression expression) => this.expression = expression;

      public override void Generate(OperationsBuilder builder)
      {
         var endLabel = newLabel("end");

         expression.Generate(builder);
         builder.IsClass("InternalList", false);
         builder.GoToIfFalse(endLabel);

         builder.NewTuple();

         builder.Label(endLabel);
         builder.NoOp();
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => $"({expression})";
   }
}