using Kagami.Library.Objects;

namespace Kagami.Library.Parsers;

public abstract record PossibleMonad(TypeConstraint TypeConstraint)
{
   public sealed record Some(TypeConstraint TypeConstraint) : PossibleMonad(TypeConstraint);

   public sealed record Success(TypeConstraint TypeConstraint) : PossibleMonad(TypeConstraint);

   public sealed record None(TypeConstraint TypeConstraint) : PossibleMonad(TypeConstraint);
}