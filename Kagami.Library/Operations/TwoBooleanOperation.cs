﻿using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public abstract class TwoBooleanOperation : Operation
   {
      public abstract IMatched<bool> Execute(Machine machine, bool x, bool y);

      public override IMatched<IObject> Execute(Machine machine)
      {
         if (machine.Pop().If(out var y, out var exception) && machine.Pop().If(out var x, out exception))
            if (x is Boolean bx)
               if (y is Boolean by)
                  return Execute(machine, bx.Value, by.Value).Map(Boolean.Object);
               else
                  return failedMatch<IObject>(incompatibleClasses(y, "Boolean"));
            else
               return failedMatch<IObject>(incompatibleClasses(x, "Boolean"));
         else
            return failedMatch<IObject>(exception);
      }
   }
}