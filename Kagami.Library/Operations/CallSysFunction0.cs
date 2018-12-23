using System;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;
using Standard.Types.Monads;

namespace Kagami.Library.Operations
{
   public class CallSysFunction0 : Operation
   {
      Func<Sys, IResult<IObject>> func;
      string image;

      public CallSysFunction0(Func<Sys, IResult<IObject>> func, string image)
      {
         this.func = func;
         this.image = image;
      }

      public override IMatched<IObject> Execute(Machine machine) => func(machine.GlobalFrame.Sys).Match();

      public override string ToString() => $"call.sys.func0({image})";
   }
}