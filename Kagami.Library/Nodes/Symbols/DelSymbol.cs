using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class DelSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.PushObject(Del.Value);

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => "del";
}