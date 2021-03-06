﻿using System;
using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public readonly struct Boolean : IObject, IObjectCompare, IComparable<Boolean>, IEquatable<Boolean>, IComparable
   {
      public static implicit operator Boolean(bool value) => new(value);

      public static IObject BooleanObject(bool value) => new Boolean(value);

      public static IObject True => BooleanObject(true);

      public static IObject False => BooleanObject(false);

      private readonly bool value;

      public Boolean(bool value) : this() => this.value = value;

      public bool Value => value;

      public string ClassName => "Boolean";

      public string AsString => value ? "true" : "false";

      public string Image => AsString;

      public int Hash => value.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Boolean b && value == b.value;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => value;

      public int Compare(IObject obj) => CompareTo((Boolean)obj);

      public IObject Object => this;

      public Boolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

      public Boolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

      public int CompareTo(Boolean other) => value.CompareTo(other.value);

      public bool Equals(Boolean other) => value == other.value;

      public override bool Equals(object obj) => obj is Boolean boolean && Equals(boolean);

      public override int GetHashCode() => Hash;

      public int CompareTo(object obj) => CompareTo((Boolean)obj);
   }
}