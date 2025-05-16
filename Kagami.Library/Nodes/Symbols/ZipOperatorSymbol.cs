using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols;

public class ZipOperatorSymbol : Symbol
{
   protected Symbol operatorSymbol;

   public ZipOperatorSymbol(Symbol operatorSymbol) => this.operatorSymbol = operatorSymbol;

   public override void Generate(OperationsBuilder builder)
   {
      var _lambda = operatorLambda(operatorSymbol, builder);
      if (_lambda is (true, var lambda))
      {
         builder.PushObject(lambda);
         builder.SendMessage("zip".Selector("<Collection>", "<Lambda>"), 2);
      }
      else
      {
         throw _lambda.Exception;
      }
   }

   public override Precedence Precedence => Precedence.ChainedOperator;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => $"[{operatorSymbol}]";
}