﻿using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.Objects.ObjectFunctions;

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
			if (base.Execute(machine).If(out var value, out var original))
			{
				var valueClass = classOf(value);
				if (typeConstraint.Matches(valueClass))
					return value.Matched();
				else
					return $"You must return a type {typeConstraint.AsString}, not a {valueClass.Name}".FailedMatch<IObject>();
			}
			else
				return original;
		}

		public override string ToString() => $"return.type({typeConstraint.AsString})";
	}
}