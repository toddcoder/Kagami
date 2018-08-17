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

      public string ClassName => "Selector";

      public string AsString => image;

      public string Image => image;

      public int Hash => image.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Selector s && Equals(s);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => true;

      public bool Equals(Selector other) => image == other.image;

      public Selector Equivalent(bool[] bools)
      {
         var items = selectorItems.Select((si, i) => bools[i]? si.Equivalent(): si).ToArray();
         return new Selector(name, items, selectorImage(name, items));
      }
   }
}