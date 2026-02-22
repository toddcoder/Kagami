using Core.Collections;
using Core.Enumerables;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public struct Junction : IObject
{
   public static Junction Empty = new(JunctionType.All, []);

   private JunctionType junctionType = JunctionType.All;

   private IObject[] items = [];

   public Junction(JunctionType junctionType, IEnumerable<IObject> items)
   {
      this.junctionType = junctionType;
      this.items = [.. items];
   }

   public Junction(string junctionType, Sequence sequence)
   {
      this.junctionType = Enum.Parse<JunctionType>(junctionType, true);
      items = [.. sequence.List];
   }

   public string ClassName => "Junction";

   public string AsString => $"{junctionType.ToString().ToLower()}[{items.Select(i => i.AsString).ToString(", ")}]";

   public string Image => $"{junctionType.ToString().ToLower()}[{items.Select(i => i.Image).ToString(", ")}]";

   public int Hash => HashCode.Combine(items);

   public bool IsEqualToOther(IObject obj)
   {
      if (obj is Junction otherJunction)
      {
         return compareEnumerables(otherJunction.items, items);
      }
      else
      {
         return junctionType switch
         {
            JunctionType.All => items.All(obj.IsEqualTo),
            JunctionType.Any => items.Any(obj.IsEqualTo),
            JunctionType.One => items.Count(obj.IsEqualTo) == 1,
            JunctionType.None => items.All(i => !obj.IsEqualTo(i)),
            _ => false
         };
      }
   }

   public bool IsEqualTo(IObject obj)
   {
      if (obj is Junction otherJunction)
      {
         return compareEnumerables(items, otherJunction.items);
      }
      else
      {
         return junctionType switch
         {
            JunctionType.All => items.All(i => i.IsEqualTo(obj)),
            JunctionType.Any => items.Any(i => i.IsEqualTo(obj)),
            JunctionType.One => items.Count(i => i.IsEqualTo(obj)) == 1,
            JunctionType.None => items.All(i => !i.IsEqualTo(obj)),
            _ => false
         };
      }
   }

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => junctionType switch
   {
      JunctionType.All => items.All(i => i.Match(comparisand, bindings)),
      JunctionType.Any => items.Any(i => i.Match(comparisand, bindings)),
      JunctionType.One => items.Count(i => i.Match(comparisand, bindings)) == 1,
      JunctionType.None => items.All(i => !i.Match(comparisand, bindings)),
      _ => false
   };

   public bool IsTrue => junctionType switch
   {
      JunctionType.All => items.All(i => i.IsTrue),
      JunctionType.Any => items.Any(i => i.IsTrue),
      JunctionType.One => items.Count(i => i.IsTrue) == 1,
      JunctionType.None => items.All(i => !i.IsTrue),
      _ => false
   };

   public Guid Id { get; init; } = Guid.NewGuid();

   public Junction Append(IObject obj) => new(junctionType, items.Append(obj));

   public Junction NewJunction(IEnumerable<IObject> newItems) => new(junctionType, newItems);

   public Junction Apply(Message message)
   {
      List<IObject> mappedObjects = [];
      foreach (var item in items)
      {
         var result = sendMessage(item, message);
         mappedObjects.Add(result);
      }

      return new Junction(junctionType, mappedObjects).Flatten();
   }

   public Junction Apply(Func<IObject, IObject> application)
   {
      List<IObject> mappedObjects = [];
      foreach (var item in items)
      {
         var result = application(item);
         mappedObjects.Add(result);
      }

      return new Junction(junctionType, mappedObjects).Flatten();
   }

   public Junction Apply(Junction otherJunction, Func<IObject, IObject, IObject> application)
   {
      List<IObject> mappedObjects = [];
      foreach (var item1 in items)
      {
         foreach (var item2 in otherJunction.Items)
         {
            var result = application(item1, item2);
            mappedObjects.Add(result);
         }
      }

      return NewJunction(mappedObjects).Flatten();
   }

   public IObject[] Items => items;

   public Junction Flatten()
   {
      List<IObject> flattenedItems = [];
      foreach (var item in items)
      {
         if (item is Junction junction)
         {
            flattenedItems.AddRange(junction.Flatten().Items);
         }
         else
         {
            flattenedItems.Add(item);
         }
      }

      return new Junction(junctionType, flattenedItems);
   }
}