﻿using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
	public class TryBegin : Operation
	{
		public override IMatched<IObject> Execute(Machine machine)
		{
			machine.PushFrame(Frame.TryFrame());
			return notMatched<IObject>();
		}

		public override string ToString() => "try.begin";
	}
}