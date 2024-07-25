using System;
using Kagami.Library.Invokables;

namespace Kagami.Library.Objects
{
   public class InternalLambda : Lambda
   {
      protected Func<IObject[], IObject> func;

      public InternalLambda(Func<IObject[], IObject> func) : base(new FunctionInvokable("", Parameters.Empty, ""))
      {
         this.func = func;
      }

      public override string AsString => func.ToString();

      public override string Image => func.ToString();

      public override int Hash => func.GetHashCode();

      public override bool IsEqualTo(IObject obj) => obj is InternalLambda l && func.Equals(l.func);

      public override IObject Copy() => new InternalLambda(func);

      public override IObject Invoke(params IObject[] arguments) => func(arguments);
   }
}