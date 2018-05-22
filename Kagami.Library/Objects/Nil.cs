using System;
using Standard.Types.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public struct Nil : IObject, IOptional, IBoolean, IEquatable<Nil>
   {
      public static IObject NilValue => new Nil();

      public string ClassName => "Nil";

      public string AsString => "nil";

      public string Image => "nil";

      public int Hash => AsString.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Nil;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

	   public IObject Value => Unassigned.Value;

      public bool IsSome => false;

      public bool IsNil => true;

      public IObject Map(Lambda lambda) => this;

      public IObject FlatMap(Lambda ifSome, Lambda ifNil) => ifNil.Invoke();

      public bool IsTrue => false;

      public bool Equals(Nil other) => true;
   }
}