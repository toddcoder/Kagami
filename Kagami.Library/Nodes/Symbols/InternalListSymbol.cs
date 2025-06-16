using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class InternalListSymbol : Symbol
{
   protected Sequence sequence;

   public InternalListSymbol(Sequence sequence) => this.sequence = sequence;

   public override void Generate(OperationsBuilder builder) => builder.PushObject(sequence);

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => sequence.Image;
}