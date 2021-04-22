using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class Defer : Statement
   {
      protected Block block;

      public Defer(Block block) => this.block = block;

      public override void Generate(OperationsBuilder builder) { }

      public override string ToString() => $"defer {block}";
   }
}