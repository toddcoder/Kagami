using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers;

public abstract record PossibleBlock
{
   public sealed record Some(Block Block) : PossibleBlock
   {
      public override Maybe<Block> Maybe() => Block;
   }

   public sealed record None : PossibleBlock
   {
      public override Maybe<Block> Maybe() => nil;
   }

   public abstract Maybe<Block> Maybe();
}