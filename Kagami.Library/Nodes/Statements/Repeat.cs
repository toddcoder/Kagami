using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Kagami.Library.Parsers;

namespace Kagami.Library.Nodes.Statements;

public class Repeat : Statement
{
   protected For @for;

   public Repeat(Expression expression, Block block)
   {
      var symbol = new AnySymbol();
      List<Symbol> list = [new SubexpressionSymbol(expression, []), new SendPrefixMessage("range()")];
      var source = new Expression([.. list]);
      @for = new For(symbol, source, block, new PossibleIfExpression.None());
   }

   public override void Generate(OperationsBuilder builder) => @for.Generate(builder);

   public override string ToString() => @for.ToString();
}