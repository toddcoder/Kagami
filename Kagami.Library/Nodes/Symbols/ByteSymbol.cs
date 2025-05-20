using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class ByteSymbol : Symbol, IConstant
   {
      protected byte value;

      public ByteSymbol(byte value) => this.value = value;

      public override void Generate(OperationsBuilder builder) => builder.PushByte(value);

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => value.ToString();

      public IObject Object => (KByte)value;
   }
}