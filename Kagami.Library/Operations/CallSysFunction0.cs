using System;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class CallSysFunction0 : Operation
   {
      Func<Sys, IResult<IObject>> func;

      public CallSysFunction0(Func<Sys, IResult<IObject>> func) => this.func = func;

      public override IMatched<IObject> Execute(Machine machine) => func(machine.GlobalFrame.Sys).Match();

      public override string ToString() => "call.sys.func0";
   }
}