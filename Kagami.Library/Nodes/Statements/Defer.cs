using Kagami.Library.Invokables;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class Defer : Statement
{
   protected Block block;

   public Defer(Block block) => this.block = block;

   public override void Generate(OperationsBuilder builder)
   {
      var selector = $"__$defer{builder.Count}";
      var function = new Function(selector, Parameters.Empty, block, false, false, "");
      function.Generate(builder);
      builder.Defer($"{selector}()");
   }

   public override string ToString() => $"defer {block}";
}