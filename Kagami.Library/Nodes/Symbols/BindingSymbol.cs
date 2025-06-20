using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class BindingSymbol(string name) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.NewBinding(name);
   }

   public override Precedence Precedence => Precedence.PrefixOperator;

   public override Arity Arity => Arity.Binary;

   public override string ToString() => $"{name}'";
}