using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class OpenPackage : Statement
   {
      string packageName;

      public OpenPackage(string packageName) => this.packageName = packageName;

      public override void Generate(OperationsBuilder builder) => builder.OpenPackage(packageName);

      public override string ToString() => $"open {packageName}";
   }
}