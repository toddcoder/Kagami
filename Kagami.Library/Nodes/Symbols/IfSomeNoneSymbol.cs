using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class IfSomeNoneSymbol : Symbol
   {
      Expression result;

      public IfSomeNoneSymbol(Expression result)
      {
         this.result = result;
      }

      public override void Generate(OperationsBuilder builder)
      {
         var noneLabel = newLabel("none");
         var endLabel = newLabel("end");

         builder.GoToIfFalse(noneLabel);

         result.Generate(builder);
         builder.Some();
         builder.GoTo(endLabel);

         builder.Label(noneLabel);
         builder.PushNone();

         builder.Label(endLabel);
         builder.Peek(Index);
      }

      public override Precedence Precedence => Precedence.Boolean;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => $" || {result}";
   }
}