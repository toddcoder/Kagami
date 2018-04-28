using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class EqualSymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder) => builder.Equal();

      public override Precedence Precedence => Precedence.Boolean;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => "==";
   }
}