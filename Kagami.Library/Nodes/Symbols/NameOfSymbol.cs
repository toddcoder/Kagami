using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class NameOfSymbol(string name, bool isClass) : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.NameOf(name, isClass);

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"nameof {name}";
}