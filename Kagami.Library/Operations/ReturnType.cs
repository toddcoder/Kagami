using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
	public class ReturnType : Return
	{
		TypeConstraint typeConstraint;

		public ReturnType(bool returnTopOfStack, TypeConstraint typeConstraint) : base(returnTopOfStack)
		{
			this.typeConstraint = typeConstraint;
		}

		public override IMatched<IObject> Execute(Machine machine)
		{
			if (machine.Peek().If(out var value))
			{
				var valueClass = classOf(value);
				if (typeConstraint.Matches(valueClass))
					return base.Execute(machine);
				else
					return $"You must return a type {typeConstraint.AsString}, not a {valueClass.Name}".FailedMatch<IObject>();
			}
			else
				return failedMatch<IObject>(emptyStack());

/*			if (base.Execute(machine).If(out var value, out var original))
			{
				var valueClass = classOf(value);
				if (typeConstraint.Matches(valueClass))
					return value.Matched();
				else
					return $"You must return a type {typeConstraint.AsString}, not a {valueClass.Name}".FailedMatch<IObject>();
			}
			else
				return original;*/
		}

		public override string ToString() => $"return.type({typeConstraint.AsString})";
	}
}