using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
	public class TryEnd : Operation
	{
		public override IMatched<IObject> Execute(Machine machine)
		{
			if (machine.IsEmpty)
			{
				var result = new Objects.Success(KUnit.Value).Matched<IObject>();
				machine.PopFramesUntil(f => f.FrameType == FrameType.Try);
				return result;
			}
			else if (machine.Pop().If(out var value, out var exception))
			{
				IObject result;
				switch (value)
				{
               case Objects.Success success:
	               result = success;
						break;
               case Objects.Failure failure:
	               result = failure;
						break;
					default:
						result = new Objects.Success(value);
						break;
            }
				//machine.PopFramesUntil(f => f.FrameType == FrameType.Try);
				return result.Matched();
			}
			else
				return failedMatch<IObject>(exception);
		}

		public override string ToString() => "try.end";
	}
}