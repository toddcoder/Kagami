using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class ZipLambdaSymbol : Symbol
   {
      Expression expression;

      public ZipLambdaSymbol(Expression expression) => this.expression = expression;

      public override void Generate(OperationsBuilder builder)
      {
         expression.Generate(builder);
         builder.SendMessage("zip".Selector("<Collection>", "<Lambda>"), 2);
      }

      public override Precedence Precedence => Precedence.ChainedOperator;

      public override Arity Arity => Arity.Binary;

      public override string ToString() => $"[{expression}]";
   }
}