﻿using Core.Collections;

namespace Kagami.Library.Objects;

public readonly struct NameValue : IObject, IEquatable<NameValue>, IKeyValue
{
   private readonly string name;
   private readonly IObject value;

   public NameValue(string name, IObject value) : this()
   {
      this.name = name;
      this.value = value;
   }

   public string Name => name;

   public string ClassName => "NameValue";

   public string AsString => $"{name}: {value.AsString}";

   public string Image => $"{name}: {value.Image}";

   public int Hash => (name.GetHashCode() + value.Hash).GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is NameValue nv && name == nv.name && value.IsEqualTo(nv.value);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => value.Match(comparisand, bindings);

   public bool IsTrue => value.IsTrue;

   public Guid Id { get; init; } = Guid.NewGuid();

   public bool Equals(NameValue other) => IsEqualTo(other);

   public override bool Equals(object? obj) => obj is NameValue nameValue && Equals(nameValue);

   public override int GetHashCode() => Hash;

   public IObject Key => KString.StringObject(name);

   public IObject Value => value;

   public bool ExpandInTuple => true;
}