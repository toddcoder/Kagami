using System;
using Kagami.Library.Invokables;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using Standard.Types.Maybe;

namespace Kagami.Library.Objects
{
   public struct Comparisand : IObject, IEquatable<Comparisand>
   {
      IObject comparisand;
      IMaybe<Lambda> when;

      public Comparisand(IObject comparisand, IMaybe<Lambda> when) : this()
      {
         this.comparisand = comparisand;
         this.when = when;
      }

      public string ClassName => "Comparisand";

      public string AsString => $"&{comparisand.AsString}{when.FlatMap(i => $"when {i.Image}", () => "")}";

      public string Image => $"&{comparisand.Image}{when.FlatMap(i => $"when {i.Image}", () => "")}";

      public int Hash => comparisand.Hash;

      public bool IsEqualTo(IObject obj)
      {
         var bindings = new Hash<string, IObject>();
         if (!obj.Match(comparisand, bindings))
            return false;

         ExecutionContext.State.CurrentFields.SetBindings(bindings);
         if (when.If(out var invokable))
         {
            return invokable.
         }
      }

      public bool Match<T>(IObject comparisand, Hash<string, IObject> bindings) where T : IObject
      {
         return TODO_IMPLEMENT_ME;
      }

      public bool IsTrue { get; }

      public bool Equals(Comparisand other) => string.Equals(ClassName, other.ClassName) && string.Equals(AsString, other.AsString) && string.Equals(Image, other.Image) && Hash == other.Hash && IsTrue == other.IsTrue;

      public override bool Equals(object obj)
      {
         if (ReferenceEquals(null, obj))
            return false;

         return obj is Comparisand && Equals((Comparisand)obj);
      }

      public override int GetHashCode()
      {
         unchecked
         {
            var hashCode = (ClassName != null ? ClassName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (AsString != null ? AsString.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Image != null ? Image.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Hash;
            hashCode = (hashCode * 397) ^ IsTrue.GetHashCode();
            return hashCode;
         }
      }
   }
}