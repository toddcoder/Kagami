using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols;

public class BackPipelineSymbol : Symbol
{
   public override void Generate(OperationsBuilder builder) => builder.BackPipeline();

   public override Precedence Precedence => Precedence.Pipeline;

   public override Arity Arity => Arity.Binary;

   public override bool LeftToRight => false;

   public override string ToString() => "<|";
}