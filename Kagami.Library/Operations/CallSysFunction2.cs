using System;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations
{
   public class CallSysFunction2 : TwoOperandOperation
   {
      protected Func<Sys, IObject, IObject, IResult<IObject>> func;
      protected string image;

      public CallSysFunction2(Func<Sys, IObject, IObject, IResult<IObject>> func, string image)
      {
         this.func = func;
         this.image = image;
      }

      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         return func(machine.GlobalFrame.Sys, x, y).Match();
      }

      public override string ToString() => $"call.sys.func2({image})";
   }
}