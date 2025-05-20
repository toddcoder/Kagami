using Core.Collections;

namespace Kagami.Library.Objects;

public struct KVoid : IObject, IEquatable<KVoid>
{
   public static IObject Value => new KVoid();

   public string ClassName => "Void";

   public string AsString => "";

   public string Image => "()";

   public int Hash => ClassName.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is KVoid;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => comparisand is KVoid;

   public bool IsTrue => false;

   public bool Equals(KVoid other) => true;
}