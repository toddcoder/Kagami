﻿using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class PushFrameWithArguments : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         if (value is Arguments arguments)
         {
            var frame = new Frame(arguments);
            machine.PushFrame(frame);

            return notMatched<IObject>();
         }
         else
            return failedMatch<IObject>(incompatibleClasses(value, "Arguments"));
      }


      public override string ToString() => "push.frame.with.arguments";
   }
}