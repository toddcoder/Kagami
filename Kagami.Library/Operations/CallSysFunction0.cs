using System;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class CallSysFunction0 : Operation
{
   protected Func<Sys, Result<IObject>> func;
   protected string image;

   public CallSysFunction0(Func<Sys, Result<IObject>> func, string image)
   {
      this.func = func;
      this.image = image;
   }

   public override Optional<IObject> Execute(Machine machine) => func(machine.GlobalFrame.Sys).Optional();

   public override string ToString() => $"call.sys.func0({image})";
}