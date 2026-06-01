using Core.Collections;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Guards;

public static class Subtype
{
   private static StringHash<(LambdaSymbol lambda, Maybe<TypeConstraint> typeConstraint, Maybe<Expression> failure)> subtypes = [];

   public static Maybe<(LambdaSymbol lambda, Maybe<TypeConstraint> typeConstraint, Maybe<Expression> failure)> Get(string subtypeName) => subtypes.Maybe[subtypeName];

   public static (LambdaSymbol lambda, Maybe<TypeConstraint> typeConstraint, Maybe<Expression> failure) GetOrThrow(string subtypeName) => Get(subtypeName).Required(messageSubtypeNotFound(subtypeName));

   public static void Set(string subtypeName, LambdaSymbol lambdaSymbol, Maybe<TypeConstraint> typeConstraint, Maybe<Expression> failure)
   {
      subtypes[subtypeName] = (lambdaSymbol, typeConstraint, failure);
   }

   public static void Clear() => subtypes.Clear();
}