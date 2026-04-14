using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class Pass : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
   }

   public override string ToString() => "pass";
}