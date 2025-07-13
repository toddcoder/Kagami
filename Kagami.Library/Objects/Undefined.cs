using Core.Collections;

namespace Kagami.Library.Objects;

public readonly struct Undefined() : IObject
{
   public static IObject Value => new Undefined();

   public string ClassName => "Undefined";

   public string AsString => "undef";

   public string Image => "undef";

   public int Hash => ClassName.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Undefined;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => comparisand is Undefined;

   public bool IsTrue => false;

   public Guid Id { get; init; } = Guid.NewGuid();
}