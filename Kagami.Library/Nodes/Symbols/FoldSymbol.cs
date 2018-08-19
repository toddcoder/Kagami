using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

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
         if (operatorLambda(operatorSymbol, builder).If(out var lambda, out var exception))
         {
            builder.PushObject(lambda);
            builder.SendMessage(left ? "foldl()" : "foldr()", 1);
         }
         else
            throw exception;
      }

      public override Precedence Precedence => Precedence.SendMessage;

      public override Arity Arity => Arity.Postfix;

      public override string ToString() => $"{(left ? "/" : @"\")}{operatorSymbol}";
   }
}