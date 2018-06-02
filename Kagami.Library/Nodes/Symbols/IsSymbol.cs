using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class IsSymbol : Symbol
   {
      Expression comparisand;

      public IsSymbol(Expression comparisand) => this.comparisand = comparisand;

      public override void Generate(OperationsBuilder builder)
      {
         comparisand.Generate(builder);
         builder.Match(true, false, false);
      }

      public override Precedence Precedence => Precedence.ChainedOperator;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => $"is {comparisand}";
   }
}