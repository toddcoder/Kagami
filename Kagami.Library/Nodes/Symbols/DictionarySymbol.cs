using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
   public class DictionarySymbol : Symbol
   {
      protected Expression expression;

      public DictionarySymbol(Expression expression) => this.expression = expression;

      public override void Generate(OperationsBuilder builder)
      {
         expression.Generate(builder);
         builder.NewDictionary();
      }

      public override Precedence Precedence => Precedence.Value;

      public override Arity Arity => Arity.Nullary;

      public override string ToString() => $"<{expression}>";
   }
}