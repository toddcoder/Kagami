using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class StringSymbol : Symbol, IConstant
   {
      string value;

      public StringSymbol(string value) => this.value = value;

      public override void Generate(OperationsBuilder builder) => builder.PushString(value);

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => value;

      public IObject Object => (String)value;
   }
}