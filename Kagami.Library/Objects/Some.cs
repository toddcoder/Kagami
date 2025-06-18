using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Some : IObject, IOptional, IBoolean, IEquatable<Some>, IMonad
{
   public static IObject Object(IObject value) => new Some(value);

   private readonly IObject value;

   public Some(IObject value) : this()
   {
      this.value = value;
   }

   public string ClassName => "Some";

   public string AsString => $"{value.AsString}?";

   public string Image => $"{value.Image}?";

   public int Hash => value.Hash;

   public bool IsEqualTo(IObject obj) => obj is Some s && value.IsEqualTo(s.value);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings)
   {
      return match(this, comparisand, (s1, s2) => s1.value.Match(s2.value, bindings), bindings);
   }

   public IObject Value => value;

   public bool IsSome => true;

   public bool IsNone => false;

   public IObject Map(Lambda lambda)
   {
      var result = lambda.Invoke(value);
      return result switch
      {
         Some some => some,
         None => None.NoneValue,
         _ => new Some(result)
      };
   }

   public IObject FlatMap(Lambda ifSome, Lambda ifNone) => ifSome.Invoke(value);

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();

   public bool Equals(Some other) => value.IsEqualTo(other.value);

   public IObject Bind(Lambda map) => Map(map);

   public IObject Unit(IObject obj) => new Some(obj);

   public KBoolean CanBind => true;
}