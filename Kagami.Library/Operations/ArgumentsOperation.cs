﻿using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public abstract class ArgumentsOperation : Operation
   {
      public override IMatched<IObject> Execute(Machine machine)
      {
         if (machine.Pop().If(out var value, out var exception))
         {
            if (value is Arguments arguments)
            {
               return Execute(arguments);
            }
            else
            {
               return failedMatch<IObject>(incompatibleClasses(value, "Arguments"));
            }
         }
         else
         {
            return failedMatch<IObject>(exception);
         }
      }

      public abstract IMatched<IObject> Execute(Arguments arguments);
   }
}