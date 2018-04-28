using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class NilSymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder) => builder.PushNil();

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => "nil";
   }
}