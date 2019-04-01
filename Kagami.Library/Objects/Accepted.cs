using System;
using Core.Exceptions;

namespace Kagami.Library.Objects
{
   public class Accepted : ILazyStatus
   {
      public static ILazyStatus New(IObject obj) => new Accepted(obj);

      public Accepted(IObject obj) => Object = obj;

      public IObject Object { get; }

      public bool IsAccepted => true;

      public bool IsSkipped => false;

      public bool IsEnded => false;

      public bool IsFailed => false;

      public Exception Exception => throw "No exception".Throws();
   }
}