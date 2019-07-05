using Core.Collections;
using Core.Monads;
using Kagami.Library.Invokables;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects
{
	public class SubPattern : Pattern
	{
		protected Pattern parentPattern;
		protected IMaybe<Pattern> copyOfParentPattern;

		public SubPattern(string name, Parameters parameters, Pattern parentPattern) :
			base(name, new Lambda(new FunctionInvokable("", Parameters.Empty, "")), parameters)
		{
			this.parentPattern = parentPattern;
			copyOfParentPattern = none<Pattern>();
		}

		public IObject Value { get; set; } = Boolean.False;

		public override bool Match(IObject comparisand, Hash<string, IObject> bindings)
		{
			return returnValues(Value, bindings);
		}
	}
}