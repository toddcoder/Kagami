using System.Collections;
using System.Text;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public readonly struct SelectorItem : IEnumerable<SelectorItem>
{
   public SelectorItem(string label, Maybe<TypeConstraint> typeConstraint, SelectorItemType selectorItemType) : this()
   {
      Label = label;
      TypeConstraint = typeConstraint;
      SelectorItemType = selectorItemType;
   }

   public string Label { get; }

   public Maybe<TypeConstraint> TypeConstraint { get; }

   public SelectorItemType SelectorItemType { get; }

   public SelectorItem LabelOnly() => new(Label, nil, SelectorItemType);

   public IEnumerator<SelectorItem> GetEnumerator()
   {
      if (TypeConstraint is (true, var typeConstraint))
      {
         foreach (var tc in typeConstraint)
         {
            yield return new SelectorItem(Label, tc, SelectorItemType);
         }
      }
      else
      {
         yield return this;
      }
   }

   public override string ToString()
   {
      var builder = new StringBuilder();
      if (Label.IsNotEmpty())
      {
         builder.Append($"{Label}:");
      }

      builder.Append("_");
      if (SelectorItemType is SelectorItemType.Variadic)
      {
         builder.Append("...");
      }
      if (TypeConstraint is (true, var typeConstraint))
      {
         builder.Append(typeConstraint.Image);
      }

      return builder.ToString();
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public SelectorItem Equivalent()
   {
      var typeConstraint = TypeConstraint.Map(tc => tc.Equivalent());
      return new SelectorItem(Label, typeConstraint, SelectorItemType);
   }
}