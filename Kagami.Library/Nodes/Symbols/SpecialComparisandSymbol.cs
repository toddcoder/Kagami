using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class SpecialComparisandSymbol(SpecialComparisandDirection direction) : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.NewSpecialComparisand(direction);

   public override Precedence Precedence => Precedence.PrefixOperator;

   public override Arity Arity => Arity.Prefix;

   public override string ToString() => direction.ToString();
}