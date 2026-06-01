using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers;

public abstract record PossibleFailure
{
   public sealed record Some(Expression Failure) : PossibleFailure
   {
      public override Maybe<Expression> Maybe => Failure;
   }

   public sealed record None : PossibleFailure
   {
      public override Maybe<Expression> Maybe => nil;
   }

   public abstract Maybe<Expression> Maybe { get; }
}