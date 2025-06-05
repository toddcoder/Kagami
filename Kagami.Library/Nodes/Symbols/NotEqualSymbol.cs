using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class NotEqualSymbol : EqualSymbol
{
   public override void Generate(OperationsBuilder builder)
   {
      base.Generate(builder);
      builder.Not();
   }

   public override string ToString() => "!=";
}