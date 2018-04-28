using System;

namespace Kagami.Library.Objects
{
   public interface ILazyStatus
   {
      IObject Object { get; }

      bool IsAccepted { get; }

      bool IsSkipped { get; }

      bool IsEnded { get; }

      bool IsFailed { get; }

      Exception Exception { get; }
   }
}