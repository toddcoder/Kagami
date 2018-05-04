using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class MapOperatorSymbol : Symbol
   {
      LambdaSymbol lambdaSymbol;

      public MapOperatorSymbol(LambdaSymbol lambdaSymbol) => this.lambdaSymbol = lambdaSymbol;

      public override void Generate(OperationsBuilder builder)
      {
         lambdaSymbol.Generate(builder);
         builder.SendMessage("map", 1);
      }

      public override Precedence Precedence => Precedence.PostfixOperator;

      public override Arity Arity => Arity.Postfix;
   }
}