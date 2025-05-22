using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers;

public abstract record PossibleTypeConstraint
{
   public sealed record Some(TypeConstraint TypeConstraint) : PossibleTypeConstraint
   {
      public override Maybe<TypeConstraint> Maybe => TypeConstraint;
   }

   public sealed record None : PossibleTypeConstraint
   {
      public override Maybe<TypeConstraint> Maybe => nil;
   }

   public abstract Maybe<TypeConstraint> Maybe { get; }
};