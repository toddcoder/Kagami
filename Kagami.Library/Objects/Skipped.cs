using System;
using Standard.Types.Exceptions;

namespace Kagami.Library.Objects
{
   public class Skipped : ILazyStatus
   {
      public IObject Object => throw "No object".Throws();

      public bool IsAccepted => false;

      public bool IsSkipped => true;

      public bool IsEnded => false;

      public bool IsFailed => false;

      public Exception Exception => throw "No exception".Throws();
   }
}