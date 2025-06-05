using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class FailedMatch : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.PushString("Match failed");
      builder.Throw();
   }
}