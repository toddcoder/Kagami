using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class DefineNewField(bool mutable, string fieldName, string className, bool isHidden, bool isParam) : Statement
{
   protected bool mutable = mutable;
   protected string fieldName = fieldName;
   protected string className = className;
   protected bool isHidden = isHidden;
   protected bool isParam = isParam;

   public override void Generate(OperationsBuilder builder) => builder.DefineNewField(mutable, fieldName, className);

   public override string ToString() => $"{(mutable ? "var" : "let")} {fieldName} {className}";

   public void Deconstruct(out bool mutable, out string fieldName, out string className, out bool isHidden, out bool isParam)
   {
      mutable = this.mutable;
      fieldName = this.fieldName;
      className = this.className;
      isHidden = this.isHidden;
      isParam = this.isParam;
   }
}