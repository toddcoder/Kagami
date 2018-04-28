using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class Pass : Statement
   {
      public override void Generate(OperationsBuilder builder) => builder.PushObject(Void.Value);

      public override string ToString() => "pass";
   }
}