using Kagami.Library.Operations;
using Core.Enumerables;

namespace Kagami.Library.Nodes.Symbols
{
   public class SkipTakeOperatorSymbol : Symbol
   {
      SkipTakeItem[] arguments;

      public SkipTakeOperatorSymbol(SkipTakeItem[] arguments) => this.arguments = arguments;

/*      static void generateSkipTake(Expression expression, OperationsBuilder builder)
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
      }*/

      public override void Generate(OperationsBuilder builder)
      {
         foreach (var skipTakeItem in arguments)
            skipTakeItem.Generate(builder);
      }

      public override Precedence Precedence => Precedence.PostfixOperator;

      public override Arity Arity => Arity.Postfix;

      public override string ToString() => $"{{{arguments.Join()}}}";
   }
}