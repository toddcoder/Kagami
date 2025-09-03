using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class RepeatAsArraySymbol : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.Swap();
      builder.NewArray();
      builder.Swap();
      builder.SendMessage("*(_)", 1);
   }

   public override Precedence Precedence => Precedence.MultiplyDivide;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => "**";
}