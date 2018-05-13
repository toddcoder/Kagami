using Kagami.Library.Operations;
using Standard.Types.Enumerables;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class SkipTakeOperatorSymbol : Symbol
   {
      Expression[] count;

      public SkipTakeOperatorSymbol(Expression[] count) => this.count = count;

      public override void Generate(OperationsBuilder builder)
      {
         foreach (var expression in count)
         {
            var negativeLabel = newLabel("is-neg");
            var endLabel = newLabel("end");

            expression.Generate(builder);
            builder.Dup();
            builder.IsNegative();
            builder.GoToIfTrue(negativeLabel);

            builder.SendMessage("take", 1);
            builder.GoTo(endLabel);

            builder.Label(negativeLabel);
            builder.Negate();
            builder.SendMessage("skip", 1);

            builder.Label(endLabel);
            builder.NoOp();
         }
      }

      public override Precedence Precedence => Precedence.PostfixOperator;

      public override Arity Arity => Arity.Postfix;

      public override string ToString() => $"{{{count.Listify()}}}";
   }
}