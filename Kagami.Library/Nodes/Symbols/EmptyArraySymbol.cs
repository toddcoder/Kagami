using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class EmptyArraySymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.PushObject(KArray.Empty);

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => "{}";
}