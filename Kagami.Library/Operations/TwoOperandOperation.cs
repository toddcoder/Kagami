﻿using System;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public abstract class TwoOperandOperation : Operation
   {
      public abstract IMatched<IObject> Execute(Machine machine, IObject x, IObject y);

      public override IMatched<IObject> Execute(Machine machine)
      {
         try
         {
            if (machine.Pop().If(out var y, out var exception) && machine.Pop().If(out var x, out exception))
               return Execute(machine, x, y);
            else
               return failedMatch<IObject>(exception);
         }
         catch (Exception exception)
         {
            return failedMatch<IObject>(exception);
         }
      }
   }
}