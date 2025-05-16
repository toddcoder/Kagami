using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class FoldSymbol : Symbol
{
   protected bool left;
   protected Symbol operatorSymbol;

   public FoldSymbol(bool left, Symbol operatorSymbol)
   {
      this.left = left;
      this.operatorSymbol = operatorSymbol;
   }

   public override void Generate(OperationsBuilder builder)
   {
      var _lambda = operatorLambda(operatorSymbol, builder);
      if (_lambda is (true, var lambda))
      {
         builder.PushObject(lambda);
         builder.SendMessage(left ? "foldl()" : "foldr()", 1);
      }
      else
      {
         throw _lambda.Exception;
      }
   }

   public override Precedence Precedence => Precedence.SendMessage;

   public override Arity Arity => Arity.Postfix;

   public override string ToString() => $"{(left ? "/" : @"\")}{operatorSymbol}";
}