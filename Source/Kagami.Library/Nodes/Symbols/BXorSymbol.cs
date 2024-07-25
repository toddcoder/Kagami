using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class BXorSymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder) => builder.BXor();

      public override Precedence Precedence => Precedence.BitXOr;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => "bxor";
   }
}