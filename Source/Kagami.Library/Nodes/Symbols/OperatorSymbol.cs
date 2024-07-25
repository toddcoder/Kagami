using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class OperatorSymbol : Symbol
   {
      protected string operatorName;

      public OperatorSymbol(string operatorName) => this.operatorName = operatorName;

      public override void Generate(OperationsBuilder builder) => builder.Invoke(operatorName, 2);

      public override Precedence Precedence => Precedence.ChainedOperator;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => operatorName;
   }
}