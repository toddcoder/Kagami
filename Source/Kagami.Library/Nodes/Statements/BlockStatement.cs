using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class BlockStatement : Statement
   {
      protected Block block;

      public BlockStatement(Block block) => this.block = block;

      public override void Generate(OperationsBuilder builder)
      {
         builder.PushFrame();
         block.Generate(builder);
         builder.PopFrame();
      }

      public override string ToString() => $"block {block}";
   }
}