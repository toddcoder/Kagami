using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class WithSymbol(IEnumerable<(string property, Expression expression)> properties) : InitializerSymbol(properties)
{
   public override void Generate(OperationsBuilder builder)
   {
      builder.SendMessage("clone()", 0);
      base.Generate(builder);
   }

   public override string ToString() => $"with{base.ToString()}";
}