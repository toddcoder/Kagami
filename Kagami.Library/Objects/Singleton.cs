using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public record Singleton : IObject
{
   public static Singleton Create() => new();

   public string ClassName => "Singleton";

   public string AsString => "singleton";

   public string Image => "singleton";

   public int Hash => "singleton".GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Singleton;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => IsEqualTo(comparisand);

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();

   public Maybe<IObject> CachedValue { get; set; } = nil;
}