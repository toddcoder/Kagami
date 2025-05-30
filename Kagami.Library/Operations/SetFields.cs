﻿using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class SetFields : Operation
{
   protected Parameters parameters;

   public SetFields(Parameters parameters) => this.parameters = parameters;

   public override Optional<IObject> Execute(Machine machine)
   {
      var frame = machine.CurrentFrame;
      frame.SetFields(parameters);

      return nil;
   }

   public override string ToString() => $"set.fields({parameters})";
}