using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class PipelineSymbol : Symbol
   {
      public override void Generate(OperationsBuilder builder) => builder.Pipeline();

      public override Precedence Precedence => Precedence.ChainedOperator;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => "|>";
   }
}