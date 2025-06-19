using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class ImplicitSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
   }

   public override Precedence Precedence => Precedence.PrefixOperator;

   public override Arity Arity => Arity.Prefix;

   public override string ToString() => "^";
}