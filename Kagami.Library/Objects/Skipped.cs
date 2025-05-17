using System;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class Skipped : ILazyStatus
{
   public IObject Object => throw fail("No object");

   public bool IsAccepted => false;

   public bool IsSkipped => true;

   public bool IsEnded => false;

   public bool IsFailed => false;

   public Exception Exception => throw fail("No exception");
}