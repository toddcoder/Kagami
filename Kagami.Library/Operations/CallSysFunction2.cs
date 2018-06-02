using System;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class CallSysFunction2 : TwoOperandOperation
   {
      Func<Sys, IObject, IObject, IResult<IObject>> func;

      public CallSysFunction2(Func<Sys, IObject, IObject, IResult<IObject>> func) => this.func = func;

      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y) => func(machine.GlobalFrame.Sys, x, y).Match();

      public override string ToString() => "call.sys.func2";
   }
}