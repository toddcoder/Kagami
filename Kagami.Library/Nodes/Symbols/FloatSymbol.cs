using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class FloatSymbol : Symbol, IConstant
   {
      double value;

      public FloatSymbol(double value) => this.value = value;

      public override void Generate(OperationsBuilder builder) => builder.PushFloat(value);

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => value.ToString();

      public IObject Object => (Float)value;
   }
}