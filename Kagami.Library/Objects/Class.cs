﻿using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Class : IObject
{
   private readonly string className;

   public Class(string className) : this() => this.className = className;

   public string ClassName => className;

   public string AsString => className;

   public string Image => className;

   public int Hash => className.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Class c && className == c.className;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => true;

   public Guid Id { get; init; } = Guid.NewGuid();
}