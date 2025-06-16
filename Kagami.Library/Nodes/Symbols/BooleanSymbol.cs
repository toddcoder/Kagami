using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class BooleanSymbol : Symbol, IConstant
{
   protected bool value;

   public BooleanSymbol(bool value) => this.value = value;

   public override void Generate(OperationsBuilder builder) => builder.PushBoolean(value);

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => value.ToString().ToLower();

   public IObject Object => (KBoolean)value;
}