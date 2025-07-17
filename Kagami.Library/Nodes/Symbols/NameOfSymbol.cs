using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class NameOfSymbol(string name) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.GetField(name);
      builder.Drop();
      builder.PushString(name);
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $"nameof {name}";
}