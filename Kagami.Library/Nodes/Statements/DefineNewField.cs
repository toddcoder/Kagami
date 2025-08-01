using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class DefineNewField(bool mutable, string fieldName, string className) : Statement
{
   protected bool mutable = mutable;
   protected string fieldName = fieldName;

   public override void Generate(OperationsBuilder builder) => builder.DefineNewField(mutable, fieldName, className);

   public override string ToString() => $"{(mutable ? "var" : "let")} {fieldName} {className}";

   public void Deconstruct(out bool mutable, out string fieldName)
   {
      mutable = this.mutable;
      fieldName = this.fieldName;
   }
}