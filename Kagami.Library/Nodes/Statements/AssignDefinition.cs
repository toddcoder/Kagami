using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class AssignDefinition(string fieldName, LambdaSymbol lambdaSymbol) : Statement
{
   protected string fieldName = fieldName;
   protected LambdaSymbol lambdaSymbol = lambdaSymbol;

   public override void Generate(OperationsBuilder builder)
   {
      lambdaSymbol.Generate(builder);
      builder.NewDefinition();
      builder.NewField(fieldName, false, true);
      builder.AssignField(fieldName, false);
   }

   public override string ToString() => $"def {fieldName} = {lambdaSymbol}";

   public void Deconstruct(out string fieldName, out LambdaSymbol lambdaSymbol)
   {
      fieldName = this.fieldName;
      lambdaSymbol = this.lambdaSymbol;
   }
}