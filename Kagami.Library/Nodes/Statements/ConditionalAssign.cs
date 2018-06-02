using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Standard.Types.Maybe;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Statements
{
   public class ConditionalAssign : Statement
   {
      bool mutable;
      Symbol comparisand;
      Expression expression;
      Block block;
      IMaybe<Block> elseBlock;

      public ConditionalAssign(bool mutable, Symbol comparisand, Expression expression, Block block, IMaybe<Block> elseBlock)
      {
         this.mutable = mutable;
         this.comparisand = comparisand;
         this.expression = expression;
         this.block = block;
         this.elseBlock = elseBlock;
      }

      public override void Generate(OperationsBuilder builder)
      {
         var elseLabel = newLabel("else");
         var endLabel = newLabel("end");

         builder.PushFrame();
         expression.Generate(builder);
         comparisand.Generate(builder);
         builder.Match(mutable, true, true);
         builder.GoToIfFalse(elseBlock.IsSome ? elseLabel : endLabel);

         builder.PushFrame();
         block.Generate(builder);
         builder.PopFrame();
         builder.GoTo(endLabel);

         builder.Label(elseLabel);
         if (elseBlock.If(out var b))
         {
            builder.PushFrame();
            b.Generate(builder);
            builder.PopFrame();
         }

         builder.Label(endLabel);
         builder.PopFrame();
      }

      public override string ToString() => "";
   }
}