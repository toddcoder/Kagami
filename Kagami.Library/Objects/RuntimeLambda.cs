using System;

namespace Kagami.Library.Objects
{
   public class RuntimeLambda : Lambda
   {
      Func<IObject[], IObject> func;

      public RuntimeLambda(Func<IObject[], IObject> func, int parameterCount, string image) :
	      base(new RuntimeInvokable(parameterCount, image)) => this.func = func;

      public override IObject Invoke(params IObject[] arguments) => func(arguments);
   }
}