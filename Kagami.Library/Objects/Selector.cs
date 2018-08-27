using System;
using System.Linq;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
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
      }

      public Selector(string name) : this()
      {
         this.name = name;
         selectorItems = new SelectorItem[0];
         image = $"{name}()";
      }

      public string Name => name;

      public SelectorItem[] SelectorItems => selectorItems;

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
         var items = selectorItems.Select((si, i) => booleans[i] ? si.Equivalent(): si).ToArray();
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
					   if (right.TypeConstraint.If(out var tc) && !left.TypeConstraint.Required("Type required").IsEquivalentTo(tc))
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
   }
}