using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class PlaceholderSymbol : Symbol, IConstant
   {
      protected string name;

      public PlaceholderSymbol(string name) => this.name = name;

      public string Name => name;

      public override void Generate(OperationsBuilder builder) => builder.PushObject(new Placeholder(name));

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => $"*{name}";

      public IObject Object => new Placeholder(name);
   }
}