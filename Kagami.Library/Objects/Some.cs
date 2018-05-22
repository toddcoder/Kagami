using System;
using Standard.Types.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct Some : IObject, IOptional, IBoolean, IEquatable<Some>
   {
      public static IObject Object(IObject value) => new Some(value);

      IObject value;

      public Some(IObject value) : this() => this.value = value;

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

      public bool IsNil => false;

      public IObject Map(Lambda lambda) => lambda.Invoke(value);

      public IObject FlatMap(Lambda ifSome, Lambda ifNil) => ifSome.Invoke(value);

      public bool IsTrue => true;

      public bool Equals(Some other) => value.IsEqualTo(other.value);
   }
}