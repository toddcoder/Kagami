using System.Linq;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Core.Enumerables;
using Core.Monads;

namespace Kagami.Library.Nodes.Symbols
{
	public class MessageSymbol : Symbol
	{
		Selector selector;
		Expression[] arguments;
      IMaybe<LambdaSymbol> lambda;

		public MessageSymbol(Selector selector, Expression[] arguments, IMaybe<LambdaSymbol> lambda)
		{
			this.selector = selector;
			this.arguments = arguments;
			this.lambda = lambda;
		}

		public override void Generate(OperationsBuilder builder)
		{
			foreach (var argument in arguments)
				argument.Generate(builder);

			int count;
			if (lambda.If(out var l))
			{
				l.Generate(builder);
				count = arguments.Length + 1;
			}
			else
				count = arguments.Length;

			builder.Peek(Index);
			builder.NewMessage(selector, count);
			builder.NoOp();
      }

		public override Precedence Precedence => Precedence.Value;

		public override Arity Arity => Arity.Nullary;

		public override string ToString()
		{
			return $"?{selector}({arguments.Select(a => a.ToString()).Join()})" + lambda.FlatMap(l => $" {l}", () => "");
		}
	}
}