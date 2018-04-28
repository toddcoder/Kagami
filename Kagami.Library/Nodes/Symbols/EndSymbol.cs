using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class EndSymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder) => builder.PushObject(End.Value);

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Binary;
   }
}