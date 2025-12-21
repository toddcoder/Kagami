using Kagami.Library.Nodes.Statements;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class BlockValueSymbol(Block block) : Symbol
{
   public override void Generate(OperationsBuilder builder)
   {
      var lambdaSymbol = new LambdaSymbol(0, block, false);
      lambdaSymbol.Generate(builder);
      builder.NewDefinition();
   }

   public override Precedence Precedence => Precedence.Value;

   public override Arity Arity => Arity.Nullary;

   public override string ToString() => $".{block}";
}