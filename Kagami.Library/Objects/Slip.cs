using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Slip(IObject value) : IObject
{
   public string ClassName => "Slip";

   public string AsString => $"..{value.AsString}";

   public string Image => $"..{value.Image}";

   public int Hash => value.Hash;

   public bool IsEqualTo(IObject obj) => value.IsEqualTo(obj);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(value, comparisand, bindings);

   public bool IsTrue => value.IsTrue;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IIterator GetIterator() => value is ICollection collection ? new Iterator(collection) : new KArray(value).GetIterator(false);
}