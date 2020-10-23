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
			builder.PushFrameWithValue();

			var firstSkipTake = false;
			foreach (var skipTake in skipTakes)
			{
				if (firstSkipTake)
				{
					builder.Copy(1);
				}
				else
				{
					builder.Dup();
				}

				generate(builder, skipTake);
				if (firstSkipTake)
				{
					builder.SendMessage("~(_)", 1);
				}

				firstSkipTake = true;
			}

			builder.PopFrameWithValue();
		}

		static void generate(OperationsBuilder builder, SliceParser.SkipTake skipTake)
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

		public override Precedence Precedence => Precedence.SendMessage;

		public override Arity Arity => Arity.Postfix;

		public override string ToString() => $"{{{skipTakes.ToString(", ")}}}";
	}
}