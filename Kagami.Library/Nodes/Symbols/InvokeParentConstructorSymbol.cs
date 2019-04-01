using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class InvokeParentConstructorSymbol : InvokeSymbol
   {
      public InvokeParentConstructorSymbol(string functionName, Expression[] arguments) : base(functionName, arguments,
         none<LambdaSymbol>()) { }

      public override void Generate(OperationsBuilder builder)
      {
         base.Generate(builder);
         builder.FieldsFromObject();
      }
   }
}