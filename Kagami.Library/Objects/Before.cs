using Core.Collections;

namespace Kagami.Library.Objects;

public readonly struct Before(IObject value) : IObject, IObjectCompare
{
   public string ClassName => "Before";

   public string AsString => value.AsString;

   public string Image => value.Image;

   public int Hash => value.Hash;

   public bool IsEqualTo(IObject obj) => value.IsEqualTo(obj);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => value.Match(comparisand, bindings);

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject Value => value;

   private IObjectCompare getObjectCompare() => (IObjectCompare)value;

   public int Compare(IObject obj) => getObjectCompare().Compare(obj);

   public IObject Object => value;

   public KBoolean Between(IObject min, IObject max, bool inclusive) => getObjectCompare().Between(min, max, inclusive);

   public KBoolean After(IObject min, IObject max, bool inclusive) => getObjectCompare().After(min, max, inclusive);
}