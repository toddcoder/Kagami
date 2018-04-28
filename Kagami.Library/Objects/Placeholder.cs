using System;
using Standard.Types.Collections;

namespace Kagami.Library.Objects
{
   public struct Placeholder : IObject, IEquatable<Placeholder>
   {
      string name;

      public Placeholder(string name) : this() => this.name = name;

      public string Name => name;

      public string ClassName => "Placeholder";

      public string AsString => name;

      public string Image => $"*{name}";

      public int Hash => name.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Placeholder ph && name == ph.name;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings)
      {
         bindings[name] = comparisand;
         return true;
      }

      public bool IsTrue => true;

      public bool Equals(Placeholder other) => string.Equals(name, other.name);

      public override bool Equals(object obj) => obj is Placeholder placeholder && Equals(placeholder);

      public override int GetHashCode() => Hash;
   }
}