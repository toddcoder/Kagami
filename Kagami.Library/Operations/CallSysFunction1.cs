﻿using System;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class CallSysFunction1 : OneOperandOperation
{
   protected Func<Sys, IObject, Result<IObject>> func;
   protected string image;

   public CallSysFunction1(Func<Sys, IObject, Result<IObject>> func, string image)
   {
      this.func = func;
      this.image = image;
   }

   public override Optional<IObject> Execute(Machine machine, IObject value) => func(machine.GlobalFrame.Sys, value).Optional();

   public override string ToString() => $"call.sys.func1({image})";
}