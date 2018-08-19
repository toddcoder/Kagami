using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class MapOperatorBinarySymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder) => builder.SendMessage("map()", 1);

      public override Precedence Precedence => Precedence.ChainedOperator;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => "!";
   }
}