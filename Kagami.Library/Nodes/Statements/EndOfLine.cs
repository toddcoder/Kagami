using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class EndOfLine : Statement
   {
      public override void Generate(OperationsBuilder builder) => builder.EndOfLine();

      public override string ToString() => "eol";
   }
}