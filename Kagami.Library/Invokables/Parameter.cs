using System.Text;
using Kagami.Library.Objects;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Guards;
using Kagami.Library.Parsers;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Invokables;

public class Parameter : IEquatable<Parameter>
{
   public static Parameter New(bool isHidden, bool mutable, string name)
   {
      return new Parameter(isHidden, mutable, "", name, nil, nil, false, false, false);
   }

   protected bool isHidden;
   protected bool mutable;
   protected readonly string label;
   protected readonly string name;
   protected readonly Maybe<IInvokable> _defaultValue;
   protected readonly Maybe<TypeConstraint> _typeConstraint;
   protected readonly bool reference;
   protected readonly bool noCapturing;
   protected readonly bool lazy;
   protected readonly Maybe<Guard> _guard;

   public Parameter(bool isHidden, bool mutable, string label, string name, Maybe<IInvokable> defaultValue, Maybe<TypeConstraint> typeConstraint,
      bool reference, bool noCapturing, bool lazy)
   {
      this.isHidden = isHidden;
      this.mutable = mutable;
      this.label = label;
      this.name = name == "_" ? label : name;
      _defaultValue = defaultValue;
      _typeConstraint = typeConstraint;
      this.reference = reference;
      this.noCapturing = noCapturing;
      this.lazy = lazy;
      _guard = nil;
   }

   public Parameter(bool isHidden, bool mutable, string label, string name, PossibleInvokable defaultValue, PossibleTypeConstraint typeConstraint,
      bool reference, bool noCapturing, bool lazy, PossibleGuard guard)
   {
      this.isHidden = isHidden;
      this.mutable = mutable;
      this.label = label;
      this.name = name == "_" ? label : name;
      _defaultValue = defaultValue.Maybe;
      _typeConstraint = typeConstraint.Maybe;
      this.reference = reference;
      this.noCapturing = noCapturing;
      this.lazy = lazy;
      _guard = guard.Guard;
   }

   public bool IsHidden => isHidden;

   public bool Mutable
   {
      get => mutable;
      set => mutable = value;
   }

   public string Label => label;

   public string Name => name;

   public Maybe<IInvokable> DefaultValue => _defaultValue;

   public Maybe<TypeConstraint> TypeConstraint => _typeConstraint;

   public bool Reference => reference;

   public bool Variadic { get; set; }

   public bool NoCapturing => noCapturing;

   public bool Lazy => lazy;

   public Maybe<Guard> Guard => _guard;

   public bool Equals(Parameter? other)
   {
      return other is not null && isHidden == other.isHidden && mutable == other.mutable && string.Equals(label, other.label) &&
         string.Equals(name, other.name) && (bool)_defaultValue == (bool)other._defaultValue &&
         (bool)_typeConstraint == (bool)other._typeConstraint && reference == other.reference && lazy == other.lazy &&
         (bool)_guard == (bool)other._guard;
   }

   public override bool Equals(object? obj) => Equals((Parameter)obj!);

   public override int GetHashCode() =>
      HashCode.Combine(isHidden, name, label, _defaultValue, _typeConstraint.Map(tc => tc.Hash) | 0, reference, noCapturing, lazy);

   public string NameForFunction
   {
      get
      {
         var builder = new StringBuilder();
         if (isHidden)
         {
            builder.Append("hide ");
         }

         if (label.IsNotEmpty())
         {
            builder.Append($"{label}:");
         }

         builder.Append('_');
         if (_typeConstraint is (true, var typeConstraint))
         {
            builder.Append(typeConstraint.Image);
         }

         if (Variadic)
         {
            builder.Append("...");
         }
         else if (_defaultValue)
         {
            builder.Append('=');
         }

         return builder.ToString();
      }
   }

   public bool Singleton { get; set; }

   public override string ToString() => name;
}