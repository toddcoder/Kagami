﻿using Core.Collections;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct None : IObject, IOptional, IBoolean, IEquatable<None>, IMonad
{
   public None()
   {
   }

   public static IObject NoneValue => new None();

   public string ClassName => "None";

   public string AsString => "none";

   public string Image => "none";

   public int Hash => AsString.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is None;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public IObject Value => throw fail("No value is provided");

   public bool IsSome => false;

   public bool IsNone => true;

   public IObject Map(Lambda lambda) => this;

   public IObject FlatMap(Lambda ifSome, Lambda ifNone) => ifNone.Invoke();

   public bool IsTrue => false;

   public Guid Id { get; init; } = Guid.NewGuid();

   public bool Equals(None other) => true;

   public IObject Bind(Lambda map) => this;

   public IObject Unit(IObject obj) => this;

   public KBoolean CanBind => false;
}