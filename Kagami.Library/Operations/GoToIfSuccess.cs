﻿using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
	public class GoToIfSuccess : AddressedOperation
	{
		public override IMatched<IObject> Execute(Machine machine)
		{
			increment = false;

			if (machine.Peek().If(out var value))
			{
				if (value is IResult result)
				{
					if (result.IsSuccess)
					{
						machine.GoTo(address);
					}
					else
					{
						increment = true;
					}

					return notMatched<IObject>();
				}
				else
				{
					increment = true;
					return notMatched<IObject>();
				}
			}
			else
			{
				return failedMatch<IObject>(emptyStack());
			}
		}

		public override string ToString() => "goto.if.success";
	}
}