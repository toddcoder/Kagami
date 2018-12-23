using Kagami.Library.Operations;
using Standard.Types.Monads;

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
				builder.SendMessage("skip()", 1);
			}

			if (take.If(out var t))
			{
				t.Generate(builder);
				builder.SendMessage("take()", 1);
			}
      }

		public override Precedence Precedence => Precedence.Value;

		public override Arity Arity => Arity.Postfix;
	}
}