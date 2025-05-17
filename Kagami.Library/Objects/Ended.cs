using System;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class Ended : ILazyStatus
{
   public IObject Object => throw fail("No object");

   public bool IsAccepted => false;

   public bool IsSkipped => false;

   public bool IsEnded => true;

   public bool IsFailed => false;

   public Exception Exception => throw fail("No exception");
}