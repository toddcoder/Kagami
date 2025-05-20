using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class CharSymbol : Symbol, IConstant
   {
      protected char value;

      public CharSymbol(char value) => this.value = value;

      public override void Generate(OperationsBuilder builder) => builder.PushChar(value);

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => value.ToString();

      public IObject Object => (KChar)value;
   }
}