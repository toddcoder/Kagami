using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Statements;

namespace Kagami.Library.Nodes.Statements;

public class BuilderDo(BuilderState builderState, Block block) : BuilderStatement(builderState)
{
   public override void Generate(OperationsBuilder builder)
   {
      Prefix(builder);

      block.Generate(builder);

      Assign(builder, KUnit.Value);
   }

   public override string ToString() => $"do {{{block}}} [builder]";
}