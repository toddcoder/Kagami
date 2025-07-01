using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Unmatched : IObject
{
   public Unmatched()
   {
   }

   public static IObject Value => new Unmatched();

   public string ClassName => "Unmatched";

   public string AsString => ClassName;

   public string Image => ClassName;

   public int Hash => ClassName.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Unmatched;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => false;

   public Guid Id { get; init; } = Guid.NewGuid();
}