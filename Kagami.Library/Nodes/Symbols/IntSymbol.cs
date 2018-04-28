using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class IntSymbol : Symbol, IConstant
   {
      int value;

      public IntSymbol(int value) => this.value = value;

      public override void Generate(OperationsBuilder builder) => builder.PushInt(value);

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => value.ToString();

      public IObject Object => (Int)value;
   }
}