using System;
using Core.Collections;
using Core.Strings;

namespace Kagami.Library.Objects
{
   public readonly struct Placeholder : IObject, IEquatable<Placeholder>
   {
      private readonly string name;

      public Placeholder(string name) : this() => this.name = name;

      public string Name => name;

      public string ClassName => "Placeholder";

      public string AsString => name;

      public string Image
      {
         get
         {
            if (name.StartsWith("+"))
            {
               return $"var {name.Drop(1)}";
            }
            else if (name.StartsWith("-"))
            {
               return name.Drop(1);
            }
            else
            {
               return $"existing {name}";
            }
         }
      }

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