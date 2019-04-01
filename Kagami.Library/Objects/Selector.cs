using System;
using System.Linq;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Core.Collections;
using Core.Enumerables;
using Core.Numbers;
using Core.Strings;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
	public struct Selector : IObject, IEquatable<Selector>
	{
		public static implicit operator Selector(string source) => parseSelector(source);

		public static implicit operator string(Selector selector) => selector.image;

		string name;
		SelectorItem[] selectorItems;
		string image;

		public Selector(string name, SelectorItem[] selectorItems, string image) : this()
		{
			this.name = name;
			this.selectorItems = selectorItems;
			this.image = image;

			AnyVariadic = selectorItems.Any(si => si.SelectorItemType == SelectorItemType.Variadic);
			AnyDefault = selectorItems.Any(si => si.SelectorItemType == SelectorItemType.Default);
		}

		public Selector(string name) : this()
		{
			this.name = name;
			selectorItems = new SelectorItem[0];
			image = $"{name}()";

			AnyVariadic = false;
			AnyDefault = false;
		}

		public string Name => name;

		public SelectorItem[] SelectorItems => selectorItems;

		public bool AnyVariadic { get; }

		public bool AnyDefault { get; }

		public Selector LabelsOnly()
		{
			var items = selectorItems.Select(si => si.LabelOnly()).ToArray();
			return new Selector(name, items, $"{name}({items.Select(i => i.ToString()).Listify(",")})");
		}

		public Selector NewName(string newName) => new Selector(newName, selectorItems, selectorImage(newName, selectorItems));

		public string ClassName => "Selector";

		public string AsString => image;

		public string Image => image;

		public int Hash => image.GetHashCode();

		public bool IsEqualTo(IObject obj) => obj is Selector s && Equals(s);

		public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

		public bool IsTrue => true;

		public bool Equals(Selector other) => image == other.image;

		public Selector Equivalent(bool[] booleans)
		{
			var items = selectorItems.Select((si, i) => booleans[i] ? si.Equivalent() : si).ToArray();
			return new Selector(name, items, selectorImage(name, items));
		}

		public bool IsEquivalentTo(Selector otherSelector)
		{
			if (LabelsOnly().image == otherSelector.LabelsOnly().image)
			{
				var otherItems = otherSelector.selectorItems;
				var length = selectorItems.Length;
				if (length == otherItems.Length)
				{
					for (var i = 0; i < length; i++)
					{
						var left = selectorItems[i];
						var right = otherItems[i];
/*						if (right.TypeConstraint.If(out var tc) && !left.TypeConstraint.Required("Type required").Matches(tc))
							return false;*/
/*						if (right.TypeConstraint.If(out var rTypeConstraint) && left.TypeConstraint.If(out var lTypeConstraint))
						{
							if (!lTypeConstraint.Matches(rTypeConstraint))
								return false;
						}
						else
							return false;*/
						if (right.TypeConstraint.If(out var rTypeConstraint) && left.TypeConstraint.If(out var lTypeConstraint) && !rTypeConstraint.Matches(lTypeConstraint))
							return false;
					}

					return true;
				}
				else
					return false;
			}
			else
				return false;
		}

		public override string ToString() => image;

		public IObject Labeled(int index, IObject obj)
		{
			if (index.Between(0).Until(selectorItems.Length))
			{
				var label = selectorItems[index].Label;
				if (label.IsNotEmpty())
					return new NameValue(label, obj);
				else
					return obj;
			}
			else
				return obj;
		}

		public void Generate(int index, Expression expression, OperationsBuilder builder)
		{
			if (index.Between(0).Until(selectorItems.Length))
			{
				var label = selectorItems[index].Label;
				if (label.IsNotEmpty())
				{
					builder.PushString(label);
					expression.Generate(builder);
					builder.NewNameValue();
					return;
				}
			}

			expression.Generate(builder);
		}
	}
}