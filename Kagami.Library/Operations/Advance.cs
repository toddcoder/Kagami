﻿using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class Advance : Operation
{
   protected int increment;

   public Advance(int increment) => this.increment = increment;

   public override Optional<IObject> Execute(Machine machine)
   {
      machine.Advance(increment);
      return nil;
   }

   public override bool Increment => false;

   public override string ToString() => $"advance({increment})";
}