using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class PushFrameStatement : Statement
{
   public override void Generate(OperationsBuilder builder) => builder.PushFrame();

   public override string ToString() => "{";
}