using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class SkipTake
{
   public Maybe<Expression> Skip { get; set; } = nil;

   public Maybe<Expression> Take { get; set; } = nil;

   public bool Terminal { get; set; }

   public override string ToString()
   {
      return $"{Skip.Map(e => e.ToString()) | ""};{Take.Map(e => e.ToString()) | ""}";
   }
}