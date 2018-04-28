using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class AddSymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder) => builder.Add();

      public override Precedence Precedence => Precedence.AddSubtract;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => "+";
   }
}