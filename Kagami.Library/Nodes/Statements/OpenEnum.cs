using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class OpenEnum(string enumName) : Statement
{
   public override void Generate(OperationsBuilder builder) => builder.OpenEnum(enumName);

   public override string ToString() => $"open {enumName}";
}