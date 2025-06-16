using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class BindSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.SendMessage("bind(_<Lambda>)", 1);
   }

   public override Precedence Precedence => Precedence.ChainedOperator;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => ":-";
}