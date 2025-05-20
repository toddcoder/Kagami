using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class EmptySymbol : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;
}