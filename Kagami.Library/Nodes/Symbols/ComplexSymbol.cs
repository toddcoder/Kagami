using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class ComplexSymbol : Symbol, IConstant
   {
      double value;

      public ComplexSymbol(double value) => this.value = value;

      public override void Generate(OperationsBuilder builder) => builder.PushObject(new Complex(value));

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public IObject Object => new Complex(value);

      public override string ToString() => $"{value}i";
   }
}