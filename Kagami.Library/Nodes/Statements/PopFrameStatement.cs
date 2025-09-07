using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class PopFrameStatement : Statement
{
   public override void Generate(OperationsBuilder builder) => builder.PopFrame();

   public override string ToString() => "}";
}