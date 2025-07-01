using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class AnySymbol : Symbol, IConstant
{
   public override void Generate(OperationsBuilder builder) => builder.PushObject(Any.Value);

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => "_";

   public IObject Object => Any.Value;
}