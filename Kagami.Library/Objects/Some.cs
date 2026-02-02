using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Some : IObject, IOptional, IBoolean, IEquatable<Some>, IMonad
{
   public static IObject Object(IObject value, TypeConstraint typeConstraint) => value switch
   {
      Some some => some,
      KNil kNil => kNil,
      _ => new Some(value, typeConstraint)
   };

   private readonly IObject value;
   private readonly TypeConstraint typeConstraint;

   public Some(IObject value, TypeConstraint typeConstraint) : this()
   {
      this.value = value;
      this.typeConstraint = typeConstraint;
   }

   public TypeConstraint TypeConstraint=> typeConstraint;

   public string ClassName => "Some";

   public string AsString => $"?{value.AsString}";

   public string Image => $"?{value.Image}";

   public int Hash => value.Hash;

   public bool IsEqualTo(IObject obj) => obj is Some s && value.IsEqualTo(s.value) && typeConstraint.IsEqualTo(s.typeConstraint);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings)
   {
      return match(this, comparisand, (s1, s2) => s1.value.Match(s2.value, bindings) && s1.typeConstraint.IsEqualTo(s2.typeConstraint), bindings);
   }

   public IObject Value => value;

   public bool IsSome => true;

   public bool IsNil => false;

   public IObject Map(Lambda lambda)
   {
      var result = lambda.Invoke(value);
      return result switch
      {
         Some some => some,
         KNil or Failure => KNil.NilValue,
         Success success => new Some(success.Value, new TypeConstraint([classOf(result)])),
         _ => new Some(result, new TypeConstraint([classOf(result)]))
      };
   }

   public IObject FlatMap(Lambda ifSome, Lambda ifNone) => ifSome.Invoke(value);

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();

   public bool Equals(Some other) => value.IsEqualTo(other.value);

   public IObject Bind(Lambda map) => Map(map);

   public IObject Unit(IObject obj) => new Some(obj, new TypeConstraint([classOf(obj)]));

   public KBoolean CanBind => true;

   public IObject Result(KString message) => new Success(value, new TypeConstraint([classOf(value)]));
}