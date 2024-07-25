using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class InlineIfSymbol : Symbol
   {
      protected Expression ifTrue;
      protected Expression ifFalse;

      public InlineIfSymbol(Expression ifTrue, Expression ifFalse)
      {
         this.ifTrue = ifTrue;
         this.ifFalse = ifFalse;
      }

      public override void Generate(OperationsBuilder builder)
      {
         var falseLabel = newLabel("false");
         var endLabel = newLabel("end");

         builder.GoToIfFalse(falseLabel);

         ifTrue.Generate(builder);
         builder.GoTo(endLabel);

         builder.Label(falseLabel);
         ifFalse.Generate(builder);

         builder.Label(endLabel);
         builder.NoOp();
      }

      public override Precedence Precedence => Precedence.ChainedOperator;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => $"? {ifTrue} : {ifFalse}";
   }
}