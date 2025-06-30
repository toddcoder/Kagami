using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class ImplicitZip : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
   }

   public override Precedence Precedence => Precedence.PrefixOperator;

   public override Arity Arity => Arity.Prefix;
}