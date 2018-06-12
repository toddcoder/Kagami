using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class IfSomeNilSymbol : Symbol
   {
      Expression result;

      public IfSomeNilSymbol(Expression result)
      {
         this.result = result;
      }

      public override void Generate(OperationsBuilder builder)
      {
         var nilLabel = newLabel("nil");
         var endLabel = newLabel("end");

         builder.GoToIfFalse(nilLabel);

         result.Generate(builder);
         builder.Some();
         builder.GoTo(endLabel);

         builder.Label(nilLabel);
         builder.PushNil();

         builder.Label(endLabel);
         builder.Peek(Index);
      }

      public override Precedence Precedence => Precedence.Boolean;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => $" || {result}";
   }
}