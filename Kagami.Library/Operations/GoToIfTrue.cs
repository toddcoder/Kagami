﻿using System;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;
using Boolean = Kagami.Library.Objects.Boolean;

namespace Kagami.Library.Operations
{
   public class GoToIfTrue : AddressedOperation
   {
      protected Predicate<Boolean> predicate;

      public GoToIfTrue() => predicate = b => b.Value;

      public override IMatched<IObject> Execute(Machine machine)
      {
         increment = false;

         if (machine.Pop().If(out var x, out var exception))
            if (x is Boolean bx)
            {
               if (predicate(bx))
                  if (machine.GoTo(address))
                     return notMatched<IObject>();
                  else
                     return failedMatch<IObject>(badAddress(address));

               increment = true;
               return notMatched<IObject>();
            }
            else
               return failedMatch<IObject>(incompatibleClasses(x, "Boolean"));
         else
            return failedMatch<IObject>(exception);
      }

      public override string ToString() => $"goto.if.true({address})";
   }
}