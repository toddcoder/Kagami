using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class MultiplySymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder) => builder.Multiply();

      public override Precedence Precedence => Precedence.MultiplyDivide;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => "*";
   }
}