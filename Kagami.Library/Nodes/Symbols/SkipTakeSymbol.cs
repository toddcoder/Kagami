using Kagami.Library.Operations;
using Core.Monads;

namespace Kagami.Library.Nodes.Symbols
{
	public class SkipTakeSymbol : Symbol
	{
		protected IMaybe<Expression> skip;
		protected IMaybe<Expression> take;

		public SkipTakeSymbol(IMaybe<Expression> skip, IMaybe<Expression> take)
		{
			this.skip = skip;
			this.take = take;
		}

		public override void Generate(OperationsBuilder builder)
		{
			if (skip.If(out var s))
			{
				s.Generate(builder);
				builder.SendMessage("skip(_<Int>)", 1);
			}

			if (take.If(out var t))
			{
				t.Generate(builder);
				builder.SendMessage("take(_<Int>)", 1);
			}
      }

		public override Precedence Precedence => Precedence.SendMessage;

		public override Arity Arity => Arity.Postfix;
	}
}