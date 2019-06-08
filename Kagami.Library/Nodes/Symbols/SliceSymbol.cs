using Core.Enumerables;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Expressions;

namespace Kagami.Library.Nodes.Symbols
{
	public class SliceSymbol : Symbol
	{
		SliceParser.SkipTake[] skipTakes;

		public SliceSymbol(SliceParser.SkipTake[] skipTakes) => this.skipTakes = skipTakes;

		public override void Generate(OperationsBuilder builder)
		{
			builder.Dup();
			foreach (var skipTake in skipTakes)
			{
				builder.Dup();
				generate(builder, skipTake);
			}
		}

		void generate(OperationsBuilder builder, SliceParser.SkipTake skipTake)
		{
			if (skipTake.Skip.If(out var skipExpression))
			{
				skipExpression.Generate(builder);
				builder.SendMessage("skip(_)", 1);
			}

			if (skipTake.Take.If(out var takeExpression))
			{
				takeExpression.Generate(builder);
				builder.SendMessage("take(_)", 1);
			}
		}

		public override Precedence Precedence => Precedence.PostfixOperator;

		public override Arity Arity => Arity.Postfix;

		public override string ToString() => $"{{{skipTakes.Stringify()}}}";
	}
}