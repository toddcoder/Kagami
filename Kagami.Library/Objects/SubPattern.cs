using Core.Collections;
using Kagami.Library.Invokables;

namespace Kagami.Library.Objects
{
	public class SubPattern : Pattern
	{
		protected Pattern parentPattern;

		public SubPattern(string name, Parameters parameters, Pattern parentPattern) :
			base(name, new Lambda(new FunctionInvokable("", Parameters.Empty, "")), parameters)
		{
			this.parentPattern = parentPattern;
		}

		public override string Image => $"pattern {parentPattern.Name}[{name}]";

		public override bool IsEqualTo(IObject obj)
		{
			return obj is SubPattern subPattern && name == subPattern.name && parentPattern.IsEqualTo(subPattern.parentPattern);
		}

		public override Pattern Copy() => new SubPattern(name, parameters, parentPattern.Copy());

		public override void RegisterArguments(Arguments arguments)
		{
			parentPattern.RegisterArguments(arguments);
			base.RegisterArguments(arguments);
		}

		public override bool Match(IObject comparisand, Hash<string, IObject> bindings)
		{
			if (parentPattern.Match(comparisand, bindings))
			{
				if (IsEqualTo(parentPattern.Value))
				{
					return returnValues(parentPattern.Value, bindings);
				}
			}

			Value = Boolean.False;
			return false;
		}
	}
}