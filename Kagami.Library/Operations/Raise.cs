﻿using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class Raise : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      try
      {
         if (x is INumeric n1 && y is INumeric n2)
         {
            return n1.Raise(n2).Just();
         }
         else
         {
            return sendMessage(x, "^(_)", y).Just();
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public override string ToString() => "raise";
}