using System.Text;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Objects
{
   public struct SelectorItem
   {
      public SelectorItem(string label, IMaybe<TypeConstraint> typeConstraint) : this()
      {
         Label = label;
         TypeConstraint = typeConstraint;
      }

      public string Label { get; }

      public IMaybe<TypeConstraint> TypeConstraint { get; }

      public SelectorItem LabelOnly() => new SelectorItem(Label, none<TypeConstraint>());

      public override string ToString()
      {
         var builder = new StringBuilder();
         if (Label.IsNotEmpty())
            builder.Append($"{Label}:");
         if (TypeConstraint.If(out var tc))
            builder.Append(tc.Image);

         return builder.ToString();
      }

      public SelectorItem Equivalent()
      {
         var typeConstraint = TypeConstraint.Map(tc => tc.Equivalent());
         return new SelectorItem(Label, typeConstraint);
      }
   }
}