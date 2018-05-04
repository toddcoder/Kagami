using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class FoldSymbol : Symbol
   {
      bool left;
      Symbol operatorSymbol;

      public FoldSymbol(bool left, Symbol operatorSymbol)
      {
         this.left = left;
         this.operatorSymbol = operatorSymbol;
      }

      public override void Generate(OperationsBuilder builder)
      {
         if (NodeFunctions.operatorLambda(operatorSymbol, builder).If(out var lambda, out var exception))
         {
            builder.PushObject(lambda);
            builder.SendMessage(left ? "foldl" : "foldr", 1);
         }
         else
            throw exception;
      }

      public override Precedence Precedence => Precedence.PostfixOperator;

      public override Arity Arity => Arity.Postfix;

      public override string ToString() => $"{(left ? "/" : @"\")}{operatorSymbol}";
   }
}