using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class DefineNewField(bool mutable, string fieldName, string className) : Statement
{
   public override void Generate(OperationsBuilder builder) => builder.DefineNewField(mutable, fieldName, className);

   public override string ToString() => $"{(mutable ? "var" : "let")} {fieldName} {className}";
}