using System;
using Kagami.Library.Invokables;
using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public class Constructor : IObject, IEquatable<Constructor>, IInvokableObject
   {
      IInvokable invokable;

      public Constructor(IInvokable invokable) => this.invokable = invokable;

      public string ClassName => "Constructor";

      public string AsString => invokable.ToString();

      public string Image => invokable.Image;

      public int Hash => invokable.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Constructor c && invokable.Index == c.invokable.Index;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => true;

      public bool Equals(Constructor other) => IsEqualTo(other);

      public IInvokable Invokable => invokable;
   }
}