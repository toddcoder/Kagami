using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class NoOp : Statement
   {
      public override void Generate(OperationsBuilder builder) => builder.NoOp();

      public override string ToString() => "NoOp";
   }
}