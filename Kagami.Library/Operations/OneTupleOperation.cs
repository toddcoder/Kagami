﻿using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public abstract class OneTupleOperation : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         if (value is Tuple tuple)
            return Execute(machine, tuple);
         else
            return failedMatch<IObject>(incompatibleClasses(value, "Tuple"));
      }

      public abstract IMatched<IObject> Execute(Machine machine, Tuple tuple);
   }
}