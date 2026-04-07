using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class DefineNewField(bool mutable, string fieldName, TypeConstraint typeConstraint, bool isHidden, bool isOverride, bool isParam) : Statement
{
   protected bool mutable = mutable;
   protected string fieldName = fieldName;
   protected TypeConstraint typeConstraint = typeConstraint;
   protected bool isHidden = isHidden;
   protected bool isOverride = isOverride;
   protected bool isParam = isParam;

   public override void Generate(OperationsBuilder builder) => builder.DefineNewField(mutable, fieldName, typeConstraint);

   public override string ToString() => $"{(mutable ? "var" : "let")} {fieldName} {typeConstraint}";

   public void Deconstruct(out bool mutable, out string fieldName, out TypeConstraint typeConstraint, out bool isHidden, out bool isOverride,
      out bool isParam)
   {
      mutable = this.mutable;
      fieldName = this.fieldName;
      typeConstraint = this.typeConstraint;
      isHidden = this.isHidden;
      isOverride = this.isOverride;
      isParam = this.isParam;
   }
}