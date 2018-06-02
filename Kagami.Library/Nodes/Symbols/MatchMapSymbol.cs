using Kagami.Library.Objects;
using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class MatchMapSymbol : Symbol
   {
      Symbol comparisand;
      Expression expression;

      public MatchMapSymbol(Symbol comparisand, Expression expression)
      {
         this.comparisand = comparisand;
         this.expression = expression;
      }

      public override void Generate(OperationsBuilder builder)
      {
         var elseLabel = newLabel("else");
         var endLabel = newLabel("end");

         builder.PushFrameWithValue();

         comparisand.Generate(builder);
         builder.Match(true, false, false);
         builder.GoToIfFalse(elseLabel);
         expression.Generate(builder);
         builder.GoTo(endLabel);

         builder.Label(elseLabel);
         builder.PushObject(Unmatched.Value);

         builder.Label(endLabel);
         builder.PopFrameWithValue();
      }

      public override Precedence Precedence => Precedence.PostfixOperator;

      public override Arity Arity => Arity.Postfix;

      public override string ToString() => $"|| {comparisand} || {expression}";
   }
}