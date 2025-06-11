using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct Selector : IObject, IEquatable<Selector>
{
   public static implicit operator Selector(string source) => parseSelector(source);

   public static implicit operator string(Selector selector) => selector.image;

   private readonly string name;
   private readonly SelectorItem[] selectorItems;
   private readonly string image;

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
      selectorItems = [];
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
      return new Selector(name, items, $"{name}({items.Select(i => i.ToString()).ToString(",")})".Replace("...", ""));
   }

   public Selector NewName(string newName) => new(newName, selectorItems, selectorImage(newName, selectorItems));

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
               var _result =
                  from leftConstraint in selectorItems[i].TypeConstraint
                  from rightConstraint in otherItems[i].TypeConstraint
                  select (leftConstraint, rightConstraint);
               if (_result is (true, var (left, right)) && !left.IsEquivalentTo(right))
               {
                  return false;
               }
            }

            return true;
         }
         else
         {
            return false;
         }
      }
      else
      {
         return false;
      }
   }

   public Maybe<Selector> Optional()
   {
      var self = this;
      return maybe<Selector>() & selectorItems.Length > 0 & (() => new Selector(self.name, self.selectorItems.Skip(-1).ToArray(), ""));
   }

   public override string ToString() => image;

   public IObject Labeled(int index, IObject obj)
   {
      if (index.Between(0).Until(selectorItems.Length))
      {
         var label = selectorItems[index].Label;
         return label.IsNotEmpty() ? new NameValue(label, obj) : obj;
      }
      else
      {
         return obj;
      }
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

   public IEnumerable<Selector> AllSelectors()
   {
      yield return this;

      var continuing = true;
      var length = selectorItems.Length;
      var take = length - 1;

      for (var i = length - 1; i > -1 && continuing; i--)
      {
         var items = selectorItems.Take(take--).ToArray();
         var newImage = selectorImage(name, items);
         switch (selectorItems[i].SelectorItemType)
         {
            case SelectorItemType.Variadic:
               continuing = false;
               yield return new Selector(name, items, newImage);

               break;
            case SelectorItemType.Default:
               yield return new Selector(name, items, newImage);

               break;
            default:
               continuing = false;
               break;
         }
      }
   }
}