using System.Text;
using Kagami.Library.Objects;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Parsers;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Invokables;

public class Parameter : IEquatable<Parameter>
{
   public static Parameter New(bool mutable, string name)
   {
      return new Parameter(mutable, "", name, nil, nil, false, false);
   }

   protected bool mutable;
   protected string label;
   protected string name;
   protected Maybe<IInvokable> _defaultValue;
   protected Maybe<TypeConstraint> _typeConstraint;
   protected bool reference;
   protected bool capturing;

   public Parameter(bool mutable, string label, string name, Maybe<IInvokable> defaultValue, Maybe<TypeConstraint> typeConstraint,
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

   public Parameter(bool mutable, string label, string name, PossibleInvokable defaultValue, PossibleTypeConstraint typeConstraint, bool reference,
      bool capturing)
   {
      this.mutable = mutable;
      this.label = label;
      this.name = name == "_" ? label : name;
      _defaultValue = defaultValue.Maybe;
      _typeConstraint = typeConstraint.Maybe;
      this.reference = reference;
      this.capturing = capturing;
   }

   public bool Mutable => mutable;

   public string Label => label;

   public string Name => name;

   public Maybe<IInvokable> DefaultValue => _defaultValue;

   public Maybe<TypeConstraint> TypeConstraint => _typeConstraint;

   public bool Reference => reference;

   public bool Variadic { get; set; }

   public bool Capturing => capturing;

   public bool Equals(Parameter? other)
   {
      return other is not null && mutable == other.mutable && string.Equals(label, other.label) && string.Equals(name, other.name) &&
         (bool)_defaultValue == (bool)other._defaultValue && (bool)_typeConstraint == (bool)other._typeConstraint &&
         reference == other.reference;
   }

   public override bool Equals(object? obj) => Equals((Parameter)obj!);

   public override int GetHashCode() => HashCode.Combine(name, label, _defaultValue, _typeConstraint.Map(tc => tc.Hash) | 0, reference, capturing);

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
            builder.Append("=");
         }

         return builder.ToString();
      }
   }

   public override string ToString() => name;
}