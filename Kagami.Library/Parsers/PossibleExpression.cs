using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers;

public abstract record PossibleExpression
{
   public sealed record Some(Expression Expression) : PossibleExpression
   {
      public override Maybe<Expression> Maybe => Expression;
   }

   public sealed record None() : PossibleExpression
   {
      public override Maybe<Expression> Maybe => nil;
   }

   public abstract Maybe<Expression> Maybe { get; }
}