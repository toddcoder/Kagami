﻿using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class NewLambda : Operation
   {
      IInvokable invokable;

      public NewLambda(IInvokable invokable) => this.invokable = invokable;

      public override IMatched<IObject> Execute(Machine machine)
      {
         var lambda = new Lambda(invokable);
         lambda.Capture();

         return lambda.Matched<IObject>();
      }

      public override string ToString() => $"new.lambda({invokable.Image})";
   }
}