using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers;

public abstract record PossibleAndSymbol
{
   public sealed record Some(AndSymbol AndSymbol) : PossibleAndSymbol
   {
      public override Maybe<AndSymbol> Maybe => AndSymbol;
   }

   public sealed record None : PossibleAndSymbol
   {
      public override Maybe<AndSymbol> Maybe => nil;
   }

   public abstract Maybe<AndSymbol> Maybe { get; }
}