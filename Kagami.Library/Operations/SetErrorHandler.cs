﻿using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
	public class SetErrorHandler : AddressedOperation
	{
		public override IMatched<IObject> Execute(Machine machine)
		{
			increment = true;
			return machine.SetErrorHandler(address).FlatMap(u => notMatched<IObject>(), failedMatch<IObject>);
		}

		public override string ToString() => "set.error.handler";
	}
}