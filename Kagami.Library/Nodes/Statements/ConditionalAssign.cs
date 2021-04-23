using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Core.Monads;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements
{
   public class ConditionalAssign : Statement
   {
      protected Symbol comparisand;
      protected Expression expression;
      protected Block block;
      protected IMaybe<Block> _elseBlock;

      public ConditionalAssign(Symbol comparisand, Expression expression, Block block, IMaybe<Block> elseBlock)
      {
         this.comparisand = comparisand;
         this.expression = expression;
         this.block = block;
         _elseBlock = elseBlock;
      }

      public override void Generate(OperationsBuilder builder)
      {
         var elseLabel = newLabel("else");
         var endLabel = newLabel("end");

         builder.PushFrame();
         expression.Generate(builder);
         comparisand.Generate(builder);
         builder.Match();
         builder.GoToIfFalse(_elseBlock.IsSome ? elseLabel : endLabel);

         builder.PushFrame();
         block.Generate(builder);
         builder.PopFrame();
         builder.GoTo(endLabel);

         builder.Label(elseLabel);
         if (_elseBlock.If(out var elseBlock))
         {
            builder.PushFrame();
            elseBlock.Generate(builder);
            builder.PopFrame();
         }

         builder.Label(endLabel);
         builder.PopFrame();
      }

      public override string ToString() => "";
   }
}