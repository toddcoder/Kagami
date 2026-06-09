using Core.Monads;
using Kagami.Library.Guards;
using Kagami.Library.Invokables;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers;

public abstract record PossibleGuard
{
   public sealed record Some(IInvokable GuardInvokable, Maybe<IInvokable> Failure) : PossibleGuard
   {
      public override Maybe<Guard> Guard => new Guard(GuardInvokable, Failure);
   }

   public sealed record None : PossibleGuard
   {
      public override Maybe<Guard> Guard => nil;
   }

   public abstract Maybe<Guard> Guard { get; }
}