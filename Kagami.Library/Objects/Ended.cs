using System;
using Standard.Types.Exceptions;

namespace Kagami.Library.Objects
{
   public class Ended : ILazyStatus
   {
      public IObject Object => throw "No object".Throws();

      public bool IsAccepted => false;

      public bool IsSkipped => false;

      public bool IsEnded => true;

      public bool IsFailed => false;

      public Exception Exception => throw "No exception".Throws();
   }
}