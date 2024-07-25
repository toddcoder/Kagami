using System.Numerics;
using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class LongSymbol : Symbol, IConstant
   {
      protected BigInteger value;

      public LongSymbol(BigInteger value) => this.value = value;

      public override void Generate(OperationsBuilder builder) => builder.PushObject((Long)value);

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public IObject Object => (Long)value;
   }
}