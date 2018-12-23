using System;
using System.Text;
using Kagami.Library.Objects;
using Standard.Types.Monads;
using Standard.Types.Strings;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Invokables
{
   public class Parameter : IEquatable<Parameter>
   {
      public static Parameter New(bool mutable, string name)
      {
         return new Parameter(mutable, "", name, none<IInvokable>(), none<TypeConstraint>(), false, false);
      }

      bool mutable;
      string label;
      string name;
      IMaybe<IInvokable> defaultValue;
      IMaybe<TypeConstraint> typeConstraint;
      bool reference;
      bool capturing;

      public Parameter(bool mutable, string label, string name, IMaybe<IInvokable> defaultValue, IMaybe<TypeConstraint> typeConstraint,
         bool reference, bool capturing)
      {
         this.mutable = mutable;
         this.label = label;
         this.name = name == "_" ? label : name;
         this.defaultValue = defaultValue;
         this.typeConstraint = typeConstraint;
         this.reference = reference;
         this.capturing = capturing;
      }

      public bool Mutable => mutable;

      public string Label => label;

      public string Name => name;

      public IMaybe<IInvokable> DefaultValue => defaultValue;

      public IMaybe<TypeConstraint> TypeConstraint => typeConstraint;

      public bool Reference => reference;

      public bool Variadic { get; set; }

      public bool Capturing => capturing;

      public bool Equals(Parameter other)
      {
         return mutable == other.mutable && string.Equals(label, other.label) && string.Equals(name, other.name) &&
            defaultValue.IsSome == other.defaultValue.IsSome && typeConstraint.IsSome == other.typeConstraint.IsSome &&
            reference == other.reference;
      }

      public override bool Equals(object obj) => Equals((Parameter)obj);

      public override int GetHashCode()
      {
         unchecked
         {
            var hashCode = mutable.GetHashCode();
            hashCode = hashCode * 397 ^ (name?.GetHashCode() ?? 0);
            hashCode = hashCode * 397 ^ (label?.GetHashCode() ?? 0);
            hashCode = hashCode * 397 ^ (defaultValue?.GetHashCode() ?? 0);
            hashCode = hashCode * 397 ^ typeConstraint.FlatMap(tc => tc.Hash, () => 0);
            hashCode = hashCode * 397 ^ reference.GetHashCode();
            hashCode = hashCode * 397 ^ capturing.GetHashCode();
            return hashCode;
         }
      }

      public string NameForFunction
      {
         get
         {
            var builder = new StringBuilder();
            if (label.IsNotEmpty())
               builder.Append($"{label}:");
	         builder.Append("_");
            if (typeConstraint.If(out var tc))
               builder.Append(tc.Image);
	         if (Variadic)
		         builder.Append("...");
				else if (defaultValue.IsSome)
		         builder.Append("=");

            return builder.ToString();
         }
      }

      public override string ToString()
      {
         return (StringStream)"" / (reference, "ref ") / (mutable, "var ") / label.Extend(after: ": ") / name
            / typeConstraint.FlatMap(t => t.Image, () => "") / defaultValue.FlatMap(i => $" = {i.Image}", () => "");
      }
   }
}