﻿using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public abstract class OneBooleanOperation : Operation
   {
      public abstract IMatched<bool> Execute(Machine machine, bool boolean);

      public override IMatched<IObject> Execute(Machine machine)
      {
         if (machine.Pop().If(out var value, out var exception))
            if (value is Boolean b)
               return Execute(machine, b.Value).Map(Boolean.Object);
            else
               return failedMatch<IObject>(incompatibleClasses(value, "Boolean"));
         else
            return failedMatch<IObject>(exception);
      }
   }
}