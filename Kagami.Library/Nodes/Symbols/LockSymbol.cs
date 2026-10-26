using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class LockSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.Lock();

   public override Precedence Precedence => Precedence.PrefixOperator;

   public override Arity Arity => Arity.Prefix;

   public override string ToString() => "#";
}