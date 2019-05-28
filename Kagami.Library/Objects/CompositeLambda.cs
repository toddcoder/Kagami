using Kagami.Library.Invokables;

namespace Kagami.Library.Objects
{
	public class CompositeLambda : Lambda
	{
		Lambda lambda2;

		public CompositeLambda(IInvokable invokable, IInvokable invokable2) : base(invokable)
		{
			lambda2 = new Lambda(invokable2);
		}

		public override IObject Invoke(params IObject[] arguments)
		{
			var result = base.Invoke(arguments);
			return lambda2.Invoke(result);
		}
	}
}