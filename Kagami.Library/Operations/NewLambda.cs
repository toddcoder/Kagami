using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class NewLambda : Operation
{
   protected IInvokable invokable;
   protected bool captures;

   public NewLambda(IInvokable invokable, bool captures)
   {
      this.invokable = invokable;
      this.captures = captures;
   }

   public override Optional<IObject> Execute(Machine machine)
   {
      var lambda = new Lambda(invokable, captures);
      lambda.Capture(machine);

      return lambda;
   }

   public override string ToString() => $"new.lambda({invokable.Image})";
}