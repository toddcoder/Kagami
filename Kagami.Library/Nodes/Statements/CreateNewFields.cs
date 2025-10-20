using Core.Enumerables;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class CreateNewFields(string[] fields, string className) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      foreach (var field in fields)
      {
         builder.DefineNewField(true, field, className);
      }
   }

   public override string ToString() => $"var {fields.ToString(", ")} {className}";

   public string[] Fields => fields;

   public string ClassName => className;
}