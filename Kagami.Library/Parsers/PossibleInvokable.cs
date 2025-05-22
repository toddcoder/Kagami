using Core.Monads;
using Kagami.Library.Invokables;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers;

public abstract record PossibleInvokable
{
   public sealed record Some(IInvokable Invokable) : PossibleInvokable
   {
      public override Maybe<IInvokable> Maybe => Invokable.Some();
   }

   public sealed record None : PossibleInvokable
   {
      public override Maybe<IInvokable> Maybe => nil;
   }

   public abstract Maybe<IInvokable> Maybe { get; }
}