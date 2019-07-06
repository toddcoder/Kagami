using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class InternalListSymbol : Symbol
   {
      Container container;

      public InternalListSymbol(Container container) => this.container = container;

      public override void Generate(OperationsBuilder builder) => builder.PushObject(container);

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => container.Image;
   }
}