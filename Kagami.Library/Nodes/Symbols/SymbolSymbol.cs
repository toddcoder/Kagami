using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class SymbolSymbol : Symbol
   {
      string name;

      public SymbolSymbol(string name) => this.name = name;

      public override void Generate(OperationsBuilder builder) => builder.PushObject(new SymbolObject(name));

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => $"`{name}";
   }
}