using System;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;
using Standard.Types.Monads;

namespace Kagami.Library.Operations
{
   public class CallSysFunction1 : OneOperandOperation
   {
      Func<Sys, IObject, IResult<IObject>> func;
      string image;

      public CallSysFunction1(Func<Sys, IObject, IResult<IObject>> func, string image)
      {
         this.func = func;
         this.image = image;
      }

      public override IMatched<IObject> Execute(Machine machine, IObject value) => func(machine.GlobalFrame.Sys, value).Match();

      public override string ToString() => $"call.sys.func1({image})";
   }
}