using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class Skip : Statement
   {
      public override void Generate(OperationsBuilder builder) => builder.PopSkipFrame();

      public override string ToString() => "skip";
   }
}