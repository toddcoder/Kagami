﻿using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public abstract class OneBooleanOperation : Operation
   {
      public abstract IMatched<bool> Execute(bool boolean);

      public override IMatched<IObject> Execute(Machine machine)
      {
         if (machine.Pop().If(out var value, out var exception))
         {
            if (value is Boolean b)
            {
               return Execute(b.Value).Map(Boolean.BooleanObject);
            }
            else
            {
               return failedMatch<IObject>(incompatibleClasses(value, "Boolean"));
            }
         }
         else
         {
            return failedMatch<IObject>(exception);
         }
      }
   }
}