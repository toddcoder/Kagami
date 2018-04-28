using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class Exit : Statement
   {
      public override void Generate(OperationsBuilder builder) => builder.PopExitFrame();

      public override string ToString() => "exit";
   }
}