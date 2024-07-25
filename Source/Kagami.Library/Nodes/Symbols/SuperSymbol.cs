using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class SuperSymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder) => builder.Super();

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => "super";
   }
}