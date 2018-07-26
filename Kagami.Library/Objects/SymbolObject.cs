using System;
using Standard.Types.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct SymbolObject : IObject, IObjectCompare, IEquatable<SymbolObject>
   {
      string name;

      public SymbolObject(string name) : this() => this.name = name;

      public string ClassName => "Symbol";

      public string AsString => name;

      public string Image => $"`{name}";

      public int Hash => name.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is SymbolObject symbol && name == symbol.name;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => true;

      public int Compare(IObject obj) => compareObjects(this, obj, (x, y) => x.name.CompareTo(y.name));

      public IObject Object => this;

      public Boolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

      public Boolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

      public bool Equals(SymbolObject other) => string.Equals(name, other.name);

      public override bool Equals(object obj) => IsEqualTo((IObject)obj);

      public override int GetHashCode() => Hash;
   }
}