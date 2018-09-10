using System.Collections;
using System.Collections.Generic;
using System.Text;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Objects
{
	public struct SelectorItem : IEnumerable<SelectorItem>
	{
		public SelectorItem(string label, IMaybe<TypeConstraint> typeConstraint, SelectorItemType selectorItemType) : this()
		{
			Label = label;
			TypeConstraint = typeConstraint;
			SelectorItemType = selectorItemType;
		}

		public string Label { get; }

		public IMaybe<TypeConstraint> TypeConstraint { get; }

		public SelectorItemType SelectorItemType { get; }

		public SelectorItem LabelOnly() => new SelectorItem(Label, none<TypeConstraint>(), SelectorItemType);

		public IEnumerator<SelectorItem> GetEnumerator()
		{
			if (TypeConstraint.If(out var typeConstraint))
				foreach (var tc in typeConstraint)
					yield return new SelectorItem(Label, tc.Some(), SelectorItemType);
			else
				yield return this;
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			if (Label.IsNotEmpty())
				builder.Append($"{Label}:");
			if (TypeConstraint.If(out var tc))
				builder.Append(tc.Image);

/*			switch (SelectorItemType)
			{
				case SelectorItemType.Variadic:
					builder.Append("...");
					break;
				case SelectorItemType.Default:
					builder.Append("=");
					break;
			}*/

			return builder.ToString();
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public SelectorItem Equivalent()
		{
			var typeConstraint = TypeConstraint.Map(tc => tc.Equivalent());
			return new SelectorItem(Label, typeConstraint, SelectorItemType);
		}
	}
}