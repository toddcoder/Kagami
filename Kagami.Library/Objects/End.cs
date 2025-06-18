using Core.Collections;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct End : IObject, IObjectCompare
{
   public End()
   {
   }

   public static IObject Value => new End();

   public string ClassName => "End";

   public string AsString => "end";

   public string Image => "end";

   public int Hash => "end".GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is End;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => false;

   public Guid Id { get; init; } = Guid.NewGuid();

   public int Compare(IObject obj) => throw fail("End not replaced");

   public IObject Object => this;

   public KBoolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

   public KBoolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);
}