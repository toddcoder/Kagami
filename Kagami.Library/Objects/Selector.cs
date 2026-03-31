using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public struct Selector : IObject, IEquatable<Selector>
{
   public static implicit operator Selector(string source) => parseSelector(source);

   public static implicit operator string(Selector selector) => selector.image;

   private string name = "";
   private SelectorItem[] selectorItems = [];
   private string image = "";

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

   public IEnumerable<KTuple> GetSelectorItems()
   {
      foreach (var selectorItem in selectorItems)
      {
         var label = selectorItem.Label;
         var typeConstraint = selectorItem.TypeConstraint.Select(t => t.Comparisands.Select(c => c.Name).ToString(" ")) | "";
         var type = selectorItem.SelectorItemType switch
         {
            SelectorItemType.Normal => "normal",
            SelectorItemType.Variadic => "variadic",
            SelectorItemType.Default => "default",
            _ => "unknown"
         };
         var tuple = KTuple.Tuple3(label, typeConstraint, type);
         yield return tuple;
      }
   }

   public KArray GetSelectorItemArray() => new(GetSelectorItems().Select(t => (IObject)t));

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

   public Guid Id { get; init; } = Guid.NewGuid();

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
      List<SelectorItem> items = [.. SelectorItems];
      if (items.Count == 0)
      {
         yield return getSelector(name);

         yield break;
      }

      while (items.Count > 0)
      {
         switch (items[^1].SelectorItemType)
         {
            case SelectorItemType.Normal:
            {
               yield return getSelector(name);

               if (items[^1].TypeConstraint is (true, var typeConstraint))
               {
                  var baseClass = typeConstraint.Comparisands[0];
                  if (isMonad(baseClass.Name))
                  {
                     items[^1] = new SelectorItem(items[^1].Label, nil, SelectorItemType.Normal);
                     yield return getSelector(name);
                  }
               }

               yield break;
            }
            case SelectorItemType.Variadic:
            {
               yield return getSelector(name);

               items[^1] = items[^1].AsNormal();
               yield return getSelector(name);

               yield break;
            }
            case SelectorItemType.Default:
            {
               yield return getSelector(name);

               items[^1] = items[^1].AsNormal();
               yield return getSelector(name);

               items.RemoveAt(items.Count - 1);

               if (items.Count == 0)
               {
                  yield return getSelector(name);

                  items = [.. SelectorItems.Select(i => i.AsNormal())];
                  yield return getSelector(name);
               }

               break;
            }
         }
      }

      yield break;

      Selector getSelector(string name)
      {
         SelectorItem[] array = [.. items];
         var newImage = selectorImage(name, array);
         return new Selector(name, array, newImage);
      }
   }

   public IObject Assign(Lambda lambda)
   {
      Machine.Current.Assign(this, lambda, true);
      return this;
   }

   public Selector WithVariadic()
   {
      if (selectorItems.Length > 0)
      {
         var lastItem = selectorItems[^1];
         selectorItems[^1] = lastItem.AsVariadic();
         return new Selector(name, selectorItems, selectorImage(name, selectorItems));
      }
      else
      {
         return new Selector();
      }
   }
}