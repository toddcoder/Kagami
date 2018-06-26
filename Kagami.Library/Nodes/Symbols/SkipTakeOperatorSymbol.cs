using Kagami.Library.Operations;
using Standard.Types.Enumerables;
using Standard.Types.Exceptions;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class SkipTakeOperatorSymbol : Symbol
   {
      Expression[] arguments;

      public SkipTakeOperatorSymbol(Expression[] arguments) => this.arguments = arguments;

      void generateSkipTake(Expression expression, OperationsBuilder builder)
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

      public override void Generate(OperationsBuilder builder)
      {
         switch (arguments.Length)
         {
            case 1:
               generateSkipTake(arguments[0], builder);
               break;
            case 2:
               generateSkipTake(arguments[0], builder);
               generateSkipTake(arguments[1], builder);
               break;
            case 3:
               var uniqueFieldName = newLabel("collection");
               builder.NewField(uniqueFieldName, false, true);
               builder.AssignField(uniqueFieldName, false);

               builder.GetField(uniqueFieldName);
               generateSkipTake(arguments[0], builder);

               arguments[1].Generate(builder);
               builder.SendMessage("~", 1);

               builder.GetField(uniqueFieldName);
               generateSkipTake(arguments[2], builder);
               builder.SendMessage("~", 1);
               break;
            default:
               throw "2 or 3 arguments required".Throws();
         }
      }

      public override Precedence Precedence => Precedence.PostfixOperator;

      public override Arity Arity => Arity.Postfix;

      public override string ToString() => $"{{{arguments.Listify()}}}";
   }
}