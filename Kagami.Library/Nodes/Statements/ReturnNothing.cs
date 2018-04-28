using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class ReturnNothing : Statement
   {
      public override void Generate(OperationsBuilder builder) => builder.Return(false);

      public override string ToString() => "return";
   }
}