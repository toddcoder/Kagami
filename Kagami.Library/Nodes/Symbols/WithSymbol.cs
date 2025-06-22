using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class WithSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.SendMessage("with(_)", 1);
   }

   public override Precedence Precedence => Precedence.ChainedOperator;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "with";
}