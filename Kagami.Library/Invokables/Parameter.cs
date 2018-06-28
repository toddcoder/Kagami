using System;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Invokables
{
   public class Parameter : IEquatable<Parameter>
   {
      public static Parameter New(bool mutable, string name) => new Parameter(mutable, "", name, none<IInvokable>(), false);

      bool mutable;
      string label;
      string name;
      IMaybe<IInvokable> defaultValue;
      bool reference;

      public Parameter(bool mutable, string label, string name, IMaybe<IInvokable> defaultValue, bool reference)
      {
         this.mutable = mutable;
         this.label = label;
         this.name = name == "_" ? label : name;
         this.defaultValue = defaultValue;
         this.reference = reference;
      }

      public bool Mutable => mutable;

      public string Label => label;

      public string Name => name;

      public IMaybe<IInvokable> DefaultValue => defaultValue;

      public bool Reference => reference;

      public bool Variadic { get; set; }

      public bool Equals(Parameter other)
      {
         return mutable == other.mutable && string.Equals(label, other.label) && string.Equals(name, other.name) &&
            Equals(defaultValue, other.defaultValue) && reference == other.reference;
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
            hashCode = hashCode * 397 ^ reference.GetHashCode();
            return hashCode;
         }
      }

      public string NameForFunction => label.DefaultTo("");

      public override string ToString()
      {
         return (StringStream)"" / (reference, "ref ") / (mutable, "var ") / label.Extend(after: ": ") / name /
            defaultValue.FlatMap(i => $" = {i.Image}", () => "");
      }
   }
}