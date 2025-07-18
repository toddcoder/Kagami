using Kagami.Library.Invokables;

namespace Kagami.Library.Objects;

public class CompositeLambda : Lambda
{
   protected Lambda lambda1;
   protected Lambda lambda2;

   public CompositeLambda(IInvokable invokable, IInvokable invokable2) : base(invokable, true)
   {
      lambda1 = new Lambda(invokable, true);
      lambda2 = new Lambda(invokable2, true);
   }

   public override IObject Invoke(params IObject[] arguments)
   {
      var result = lambda2.Invoke(arguments);
      return lambda1.Invoke(result);
   }
}