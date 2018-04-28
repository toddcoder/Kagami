using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class InternalListSymbol : Symbol
   {
      InternalList internalList;

      public InternalListSymbol(InternalList internalList) => this.internalList = internalList;

      public override void Generate(OperationsBuilder builder) => builder.PushObject(internalList);

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => internalList.Image;
   }
}