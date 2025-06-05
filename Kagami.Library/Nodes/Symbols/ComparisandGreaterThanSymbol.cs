using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class ComparisandGreaterThanSymbol : GreaterThanSymbol, ISpecialComparisand
{
   public string FieldName { get; set; } = "";

   public override void Generate(OperationsBuilder builder)
   {
      builder.GetField(FieldName);
      builder.Swap();
      base.Generate(builder);
   }

   public override Precedence Precedence => Precedence.PrefixOperator;
}