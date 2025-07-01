using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class Reassignment(string fieldName, Expression expression) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      Symbol[] symbols = [new FieldSymbol(fieldName), ..expression.Symbols];
      foreach (var symbol in symbols)
      {
         symbol.Generate(builder);
      }
      builder.AssignField(fieldName, true);
   }

   public override string ToString() => $"{fieldName} .= {expression}";
}