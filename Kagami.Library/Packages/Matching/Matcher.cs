using Core.Collections;
using Kagami.Library.Objects;

namespace Kagami.Library.Packages.Matching;

public abstract class Matcher : IObject
{
   public abstract string ClassName { get; }

   public abstract string AsString { get; }

   public abstract string Image { get; }

   public abstract int Hash { get; }

   public abstract bool IsEqualTo(IObject obj);

   public abstract bool Match(IObject comparisand, Hash<string, IObject> bindings);

   public abstract bool IsTrue { get; }

   public Guid Id { get; init; } = Guid.NewGuid();
}