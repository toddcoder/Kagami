using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class IteratorSymbol : Symbol
   {
      bool lazy;

      public IteratorSymbol(bool lazy) => this.lazy = lazy;

      public override void Generate(OperationsBuilder builder) => builder.GetIterator(lazy);

      public override Precedence Precedence => Precedence.PrefixOperator;

      public override Arity Arity => Arity.Prefix;

      public override string ToString() => lazy ? "^" : "!";
   }
}