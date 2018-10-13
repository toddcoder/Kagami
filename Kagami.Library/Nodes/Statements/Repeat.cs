using System.Collections.Generic;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
	public class Repeat : Statement
	{
		For2 for2;

		public Repeat(Expression expression, Block block)
		{
			var symbol=new AnySymbol();
			var list = new List<Symbol> { new SubexpressionSymbol(expression), new SendPrefixMessage("range()") };
			var source = new Expression(list.ToArray());
			for2 = new For2(symbol, source, block);
		}

		public override void Generate(OperationsBuilder builder) => for2.Generate(builder);

		public override string ToString() => for2.ToString();
	}
}