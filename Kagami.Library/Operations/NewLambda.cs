﻿using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class NewLambda : Operation
{
   protected IInvokable invokable;

   public NewLambda(IInvokable invokable) => this.invokable = invokable;

   public override Optional<IObject> Execute(Machine machine)
   {
      var lambda = new Lambda(invokable);
      lambda.Capture();

      return lambda;
   }

   public override string ToString() => $"new.lambda({invokable.Image})";
}