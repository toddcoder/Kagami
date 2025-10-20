using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class DefinedSymbol(string name, bool isClass) : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.Defined(name, isClass);

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"defined {name}";
}