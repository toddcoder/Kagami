﻿using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class SetErrorHandler : AddressedOperation
   {
      public override IMatched<IObject> Execute(Machine machine)
      {
         increment = true;
         return machine.SetErrorHandler(address).Map(_ => notMatched<IObject>()).Recover(failedMatch<IObject>);
      }

      public override string ToString() => "set.error.handler";
   }
}