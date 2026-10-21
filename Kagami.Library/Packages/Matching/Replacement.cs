using Core.Collections;
using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Packages.Matching;

public abstract class Replacement : IObject
{
   public abstract string Text { get; }

   public abstract bool Immediate { get; set; }

   public abstract long ReplacementId { get; }

   public abstract Replacement Clone();

   public Arguments Arguments { get; set; } = Arguments.Empty;

   public Maybe<long> FixedId { get; set; } = nil;

   public abstract string ClassName { get; }

   public abstract string AsString { get; }

   public abstract string Image { get; }

   public abstract int Hash { get; }

   public abstract bool IsEqualTo(IObject obj);

   public abstract bool Match(IObject comparisand, Hash<string, IObject> bindings);

   public abstract bool IsTrue { get; }

   public Guid Id { get; init; } = Guid.NewGuid();
}