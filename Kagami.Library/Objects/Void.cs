using System;
using Core.Collections;

namespace Kagami.Library.Objects
{
   public struct Void : IObject, IEquatable<Void>
   {
      public static IObject Value => new Void();

      public string ClassName => "Void";

      public string AsString => "";

      public string Image => "()";

      public int Hash => ClassName.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Void;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => comparisand is Void;

	   public bool IsTrue => false;

      public bool Equals(Void other) => true;
   }
}