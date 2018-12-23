using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
	public class GoToIfFailure : AddressedOperation
	{
		public override IMatched<IObject> Execute(Machine machine)
		{
			increment = false;

			if (machine.Peek().If(out var value))
				if (value is IResult result)
				{
					if (result.IsFailure)
						machine.GoTo(address);
					else
						increment = true;

					return notMatched<IObject>();
				}
				else
				{
					increment = true;
					return notMatched<IObject>();
				}
			else
				return failedMatch<IObject>(emptyStack());
      }

		public override string ToString() => "goto.if.failure";
	}
}