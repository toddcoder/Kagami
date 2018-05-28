using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
   public class ImportPackage : Statement
   {
      string packageName;

      public ImportPackage(string packageName) => this.packageName = packageName;

      public override void Generate(OperationsBuilder builder) => builder.ImportPackage(packageName);

      public override string ToString() => $"import {packageName}";
   }
}