using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class AssignLambda(string fieldName, LambdaSymbol lambda) : Statement
{
   public override void Generate(OperationsBuilder builder)
   {
      lambda.Generate(builder);
      builder.LambdaCapture();
      builder.NewField(fieldName, false, true);
      builder.AssignField(fieldName, false);
   }

   public override string ToString() => $"let {fieldName}{lambda} = {lambda}";
}