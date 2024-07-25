using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class NegateSymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder) => builder.Negate();

      public override Precedence Precedence => Precedence.PrefixOperator;

      public override Arity Arity => Arity.Prefix;

      public override string ToString() => "-";
   }
}