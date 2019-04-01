using System;
using Core.Exceptions;

namespace Kagami.Library.Objects
{
   public class Failed : ILazyStatus
   {
      public Failed(Exception exception) => Exception = exception;

      public IObject Object => throw "No object".Throws();

      public bool IsAccepted => false;

      public bool IsSkipped => false;

      public bool IsEnded => false;

      public bool IsFailed => true;

      public Exception Exception { get; }
   }
}