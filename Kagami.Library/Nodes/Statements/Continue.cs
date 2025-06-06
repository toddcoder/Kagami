using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class Continue : Statement
{
   public override void Generate(OperationsBuilder builder) => builder.PopSkipFrame();

   public override string ToString() => "continue";
}