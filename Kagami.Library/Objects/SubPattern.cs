using Core.Collections;
using Kagami.Library.Invokables;

namespace Kagami.Library.Objects
{
	public class SubPattern : Pattern
	{
		public SubPattern(string name, Parameters parameters) :
			base(name, new Lambda(new FunctionInvokable("", Parameters.Empty, "")), parameters) { }

		public IObject Value { get; set; } = Boolean.False;

		public override bool Match(IObject comparisand, Hash<string, IObject> bindings)
		{
			return returnValues(Value, bindings);
		}
	}
}