using Core.Enumerables;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class CreateNewFields(string[] fields, TypeConstraint typeConstraint, bool isHidden, bool isOverride) : Statement, IFieldsStatement
{
   public override void Generate(OperationsBuilder builder)
   {
      foreach (var field in fields)
      {
         builder.DefineNewField(true, field, typeConstraint);
      }
   }

   public override string ToString() => $"var {fields.ToString(", ")} {typeConstraint}";

   public IEnumerable<IFieldStatement> FieldStatements() =>
      fields.Select(field => new DefineNewField(true, field, typeConstraint, isHidden, isOverride, false));

   public string[] Fields => fields;

   public string ClassName => typeConstraint.Comparisands[0].Name;

   public bool IsHidden => isHidden;

   public bool IsOverride => isOverride;
}