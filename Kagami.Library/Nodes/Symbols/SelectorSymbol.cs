using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class SelectorSymbol(Selector selector) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.PushObject(selector);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => selector.AsString;
}