using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class Failed : ILazyStatus
{
   public Failed(Exception exception) => Exception = exception;

   public IObject Object => throw fail("No object");

   public bool IsAccepted => false;

   public bool IsSkipped => false;

   public bool IsEnded => false;

   public bool IsFailed => true;

   public Exception Exception { get; }
}