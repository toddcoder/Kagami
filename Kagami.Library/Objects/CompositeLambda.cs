using Kagami.Library.Invokables;

namespace Kagami.Library.Objects
{
	public class CompositeLambda : Lambda
	{
		Lambda lambda1;
		Lambda lambda2;

		public CompositeLambda(IInvokable invokable1, IInvokable invokable2) : base(invokable1)
		{
			lambda1 = new Lambda(invokable1);
			lambda2 = new Lambda(invokable2);
		}

		public override IObject Invoke(params IObject[] arguments)
		{
			var result = lambda2.Invoke(arguments);
			return lambda1.Invoke(result);
		}
	}
}