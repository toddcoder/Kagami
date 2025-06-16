using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class InvokeParentConstructorSymbol : InvokeSymbol
{
   public InvokeParentConstructorSymbol(string functionName, Expression[] arguments, bool inComparisand) :
      base(functionName, arguments, nil, inComparisand)
   {
   }

   public override void Generate(OperationsBuilder builder)
   {
      base.Generate(builder);
      builder.FieldsFromObject();
   }
}