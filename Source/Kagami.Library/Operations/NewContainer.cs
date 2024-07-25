using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
	public class NewContainer : Operation
	{
		public override IMatched<IObject> Execute(Machine machine)
		{
			if (machine.Pop().If(out var y, out var exception))
			{
				if (machine.IsEmpty)
				{
					return new Tuple(y).Matched<IObject>();
				}
				else if (machine.Pop().If(out var x, out exception))
				{
					if (x is Container container)
					{
						container.Add(y);
						return container.Matched<IObject>();
					}
					else
					{
						return new Container(x, y).Matched<IObject>();
					}
				}
				else
				{
					return failedMatch<IObject>(exception);
				}
			}
			else
			{
				return failedMatch<IObject>(exception);
			}
		}

		public override string ToString() => "new.container";
	}
}