using System;
using System.Text;
using Kagami.Library.Objects;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Invokables
{
   public class Parameter : IEquatable<Parameter>
   {
      public static Parameter New(bool mutable, string name)
      {
         return new(mutable, "", name, none<IInvokable>(), none<TypeConstraint>(), false, false);
      }

      protected bool mutable;
      protected string label;
      protected string name;
      protected IMaybe<IInvokable> _defaultValue;
      protected IMaybe<TypeConstraint> _typeConstraint;
      protected bool reference;
      protected bool capturing;

      public Parameter(bool mutable, string label, string name, IMaybe<IInvokable> defaultValue, IMaybe<TypeConstraint> typeConstraint,
         bool reference, bool capturing)
      {
         this.mutable = mutable;
         this.label = label;
         this.name = name == "_" ? label : name;
         _defaultValue = defaultValue;
         _typeConstraint = typeConstraint;
         this.reference = reference;
         this.capturing = capturing;
      }

      public bool Mutable => mutable;

      public string Label => label;

      public string Name => name;

      public IMaybe<IInvokable> DefaultValue => _defaultValue;

      public IMaybe<TypeConstraint> TypeConstraint => _typeConstraint;

      public bool Reference => reference;

      public bool Variadic { get; set; }

      public bool Capturing => capturing;

      public bool Equals(Parameter other)
      {
         return mutable == other.mutable && string.Equals(label, other.label) && string.Equals(name, other.name) &&
            _defaultValue.IsSome == other._defaultValue.IsSome && _typeConstraint.IsSome == other._typeConstraint.IsSome &&
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
            hashCode = hashCode * 397 ^ (_defaultValue?.GetHashCode() ?? 0);
            hashCode = hashCode * 397 ^ _typeConstraint.Map(tc => tc.Hash).DefaultTo(() => 0);
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
            {
               builder.Append($"{label}:");
            }

            builder.Append("_");
            if (_typeConstraint.If(out var tc))
            {
               builder.Append(tc.Image);
            }

            if (Variadic)
            {
               builder.Append("...");
            }
            else if (_defaultValue.IsSome)
            {
               builder.Append("=");
            }

            return builder.ToString();
         }
      }

      public override string ToString()
      {
         return (StringStream)"" / (reference, "ref ") / (mutable, "var ") / label.Map(l => $"{l}: ") / name
            / _typeConstraint.Map(t => t.Image).DefaultTo(() => "") / _defaultValue.Map(i => $" = {i.Image}").DefaultTo(() => "");
      }
   }
}