using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
   public class ZipOperatorSymbol : Symbol
   {
      Symbol operatorSymbol;

      public ZipOperatorSymbol(Symbol operatorSymbol) => this.operatorSymbol = operatorSymbol;

      public override void Generate(OperationsBuilder builder)
      {
         if (operatorLambda(operatorSymbol, builder).If(out var lambda, out var exception))
         {
            builder.PushObject(lambda);
            builder.SendMessage("zip".Function("on", "with"), 2);
         }
         else
            throw exception;
      }

      public override Precedence Precedence => Precedence.ChainedOperator;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => $"[{operatorSymbol}]";
   }
}