using Core.Collections;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct KNil : IObject, IOptional, IBoolean, IEquatable<KNil>, IMonad
{
   public KNil()
   {
   }

   public static IObject NilValue => new KNil();

   public string ClassName => "Nil";

   public string AsString => "nil";

   public string Image => "nil";

   public int Hash => AsString.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is KNil;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public IObject Value => throw fail("No value is provided");

   public bool IsSome => false;

   public bool IsNone => true;

   public IObject Map(Lambda lambda) => this;

   public IObject FlatMap(Lambda ifSome, Lambda ifNone) => ifNone.Invoke();

   public IObject Result(KString message) => new Failure(message.Value);

   public bool IsTrue => false;

   public Guid Id { get; init; } = Guid.NewGuid();

   public bool Equals(KNil other) => true;

   public IObject Bind(Lambda map) => this;

   public IObject Unit(IObject obj) => this;

   public KBoolean CanBind => false;
}