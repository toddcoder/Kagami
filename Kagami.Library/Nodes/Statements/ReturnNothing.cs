using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class ReturnNothing : Statement
   {
      public override void Generate(OperationsBuilder builder) => builder.ReturnNothing();

      public override string ToString() => "return";
   }
}