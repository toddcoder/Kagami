using System.Linq;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations
{
	public class Pipeline : TwoOperandOperation
	{
		public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
		{
			switch (y)
			{
				case Lambda lambda:
					if (x is ICollection collection)
					{
						var array = collection.GetIterator(false).List().ToArray();
						return lambda.Invoke(array).Matched();
					}
					else
					{
						return lambda.Invoke(x).Matched();
					}

				case IMayInvoke mi:
					return mi.Invoke(x).Matched();
				case Message message:
					return classOf(x).SendMessage(x, message).Matched();
				case Selector selector:
					if (Machine.Current.Find(selector).If(out var field, out var anyException))
					{
						var _ = false;
						return Invoke.InvokeObject(machine, field.Value, new Arguments(x), ref _);
					}
					else if (anyException.If(out var exception))
					{
						return failedMatch<IObject>(exception);
					}
					else
					{
						return failedMatch<IObject>(fieldNotFound(selector));
					}

				default:
					return failedMatch<IObject>(incompatibleClasses(y, "Lambda"));
			}
		}

		public override string ToString() => "pipeline";
	}
}