using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements;

public class Repeat : Statement
{
   protected For @for;

   public Repeat(Expression expression, Block block)
   {
      var symbol = new AnySymbol();
      var list = new List<Symbol> { new SubexpressionSymbol(expression), new SendPrefixMessage("range()") };
      var source = new Expression(list.ToArray());
      @for = new For(symbol, source, block);
   }

   public override void Generate(OperationsBuilder builder) => @for.Generate(builder);

   public override string ToString() => @for.ToString();
}