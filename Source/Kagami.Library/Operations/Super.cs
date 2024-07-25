using Kagami.Library.Classes;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
	public class Super : Operation
	{
		public override IMatched<IObject> Execute(Machine machine)
		{
			if (machine.Find("self", true).If(out var selfField, out var anyException))
			{
				var self = (UserObject)selfField.Value;
				var selfClass = (UserClass)classOf(self);
				var parentClassName = selfClass.ParentClassName;
				if (parentClassName.IsEmpty())
				{
					return $"Class {selfClass.Name} has no parent class".FailedMatch<IObject>();
				}
				else
				{
					var superObject = new UserObject(parentClassName, self.Fields, self.Parameters);
					return superObject.Matched<IObject>();
				}
			}
			else if (anyException.If(out var exception))
			{
				return failedMatch<IObject>(exception);
			}
			else
			{
				return "self not defined".FailedMatch<IObject>();
			}
		}

		public override string ToString() => "super";
	}
}