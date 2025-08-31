using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class AbstractFail(string functionName) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.PushString($"Abstract function {functionName} must be overriden");
      builder.Throw();
   }

   public override string ToString() => "...";
}