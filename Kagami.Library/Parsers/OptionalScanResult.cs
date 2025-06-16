using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers;

public abstract record OptionalScanResult
{
   public sealed record Value(string Text) : OptionalScanResult
   {
      public override Maybe<string> Maybe => Text;
   }

   public sealed record NoValue : OptionalScanResult
   {
      public override Maybe<string> Maybe => nil;
   }

   public abstract Maybe<string> Maybe { get; }
};