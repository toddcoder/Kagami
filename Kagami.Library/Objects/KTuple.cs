using System;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;

namespace Kagami.Library.Objects;

public readonly struct KTuple : IObject, IEquatable<KTuple>, ICollection, IObjectCompare, IComparable<KTuple>, IComparable
{
   public static IObject NewTuple(IObject x, IObject y)
   {
      if (x is KTuple t)
      {
         return new KTuple(t, y);
      }
      else
      {
         return new KTuple(x, y);
      }
   }

   public static IObject NewTupleNamed(string nameX, IObject x, string nameY, IObject y)
   {
      return new KTuple([new KeyValue(nameX, x), new KeyValue(nameY, y)]);
   }

   public static KTuple Empty => new([]);

   public static KTuple Tuple3(string left, string middle, string right)
   {
      return new([KString.StringObject(left), KString.StringObject(middle), KString.StringObject(right)]);
   }

   private readonly IObject[] items;
   private readonly Hash<string, int> names;
   private readonly Hash<int, string> indexes;

   public KTuple(IObject[] items) : this()
   {
      if (items.Length == 1 && items[0] is Container { ExpandInTuple: true } il)
      {
         this.items = il.List.ToArray();
      }
      else
      {
         this.items = items;
      }

      names = [];
      indexes = [];

      denameify();
   }

   public KTuple(IObject x, IObject y)
   {
      items = [x, y];
      names = [];
      indexes = [];

      denameify();
   }

   public KTuple(IObject value) : this([value])
   {
   }

   private void denameify()
   {
      for (var i = 0; i < items.Length; i++)
      {
         if (items[i] is IKeyValue { ExpandInTuple: true } keyValue)
         {
            names[keyValue.Key.AsString] = i;
            indexes[i] = keyValue.Key.AsString;
            items[i] = keyValue.Value;
         }
      }
   }

   public KTuple(KTuple kTuple, IObject item) : this()
   {
      var tupleItems = kTuple.items;
      var length = tupleItems.Length;

      items = new IObject[length + 1];
      System.Array.Copy(tupleItems, items, length);
      items[length] = item;

      names = new Hash<string, int>();
      indexes = new Hash<int, string>();

      denameify();
   }

   public IObject[] Value => items;

   public IObject this[int index] => items[wrapIndex(index, items.Length)];

   public IObject this[string name]
   {
      get
      {
         if (names.ContainsKey(name))
         {
            return items[names[name]];
         }
         else
         {
            throw keyNotFound((KString)name);
         }
      }
   }

   private string getItemString(int index, string text) => indexes.Maybe[index].Map(n => $"{n}: {text}") | text;

   private string getItemString(int index) => getItemString(index, items[index].AsString);

   private string getItemImage(int index) => getItemString(index, items[index].Image);

   public Maybe<IObject[]> AllButLast
   {
      get
      {
         var self = this;
         return maybe<IObject[]>() & items.Length > 0 & (() => self.items.Take(self.items.Length - 1).ToArray());
      }
   }

   public Maybe<IObject> Last
   {
      get
      {
         var self = this;
         return maybe<IObject>() & items.Length > 0 & (() => self.items[self.items.Length - 1]);
      }
   }

   public bool ContainsName(string name) => names.ContainsKey(name);

   public string ClassName => "Tuple";

   public string AsString
   {
      get
      {
         var self = this;
         return items.Select((_, i) => self.getItemString(i)).ToString(" ");
      }
   }

   public string Image
   {
      get
      {
         var self = this;
         return $"({items.Select((_, i) => self.getItemImage(i)).ToString(", ")})";
      }
   }

   public int Hash => items.GetHashCode();

   public bool IsEqualTo(IObject obj)
   {
      return obj is KTuple t && items.Length == t.items.Length &&
         items.Zip(t.items, (t1, t2) => (x: t1, y: t2)).All(tu => tu.x.IsEqualTo(tu.y));
   }

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand,
      (t1, t2) => { return t1.Length.Value == t2.Length.Value && t1.items.Zip(t2.items, (i1, i2) => i1.Match(i2, bindings)).All(b => b); },
      bindings);

   public bool IsTrue => items.Length > 0;

   public bool Equals(KTuple other) => IsEqualTo(other);

   public override bool Equals(object obj) => obj is KTuple tuple && Equals(tuple);

   public override int GetHashCode() => Hash;

   public int CompareTo(object obj)
   {
      if (obj is KTuple tuple)
      {
         return CompareTo(tuple);
      }
      else
      {
         return Compare((IObject)obj);
      }
   }

   public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

   public Maybe<IObject> Next(int index)
   {
      var self = this;
      return maybe<IObject>() & index < items.Length & (() => self.items[index]);
   }

   public Maybe<IObject> Peek(int index)
   {
      var self = this;
      return maybe<IObject>() & index < items.Length & (() => self.items[index]);
   }

   public Int Length => items.Length;

   public IEnumerable<IObject> List
   {
      get
      {
         foreach (var item in items)
         {
            yield return item;
         }
      }
   }

   public bool ExpandForArray => false;

   public KBoolean In(IObject item) => items.Contains(item);

   public KBoolean NotIn(IObject item) => !items.Contains(item);

   public IObject Times(int count)
   {
      var result = new List<IObject>();
      for (var i = 0; i < count; i++)
      {
         result.AddRange(items);
      }

      return new KTuple(result.ToArray());
   }

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public int Compare(IObject obj)
   {
      if (obj is KTuple tuple)
      {
         var tupleItems = tuple.items;
         var length = Math.Min(items.Length, tupleItems.Length);
         for (var i = 0; i < length; i++)
         {
            var left = items[i];
            var right = tupleItems[i];
            if (left is IObjectCompare lCompare)
            {
               var compare = lCompare.Compare(right);
               if (compare != 0)
               {
                  return compare;
               }
            }
         }

         return 0;
      }
      else
      {
         throw incompatibleClasses(obj, "Tuple");
      }
   }

   public IObject Object => this;

   public KBoolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

   public KBoolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

   public int CompareTo(KTuple other) => Compare(other);

   public IObject Head
   {
      get
      {
         if (items.Length == 0)
         {
            return None.NoneValue;
         }
         else
         {
            var head = items[0];
            return new Some(head);
         }
      }
   }

   public IObject Tail => items.Length == 0 ? Empty : new KTuple(items.Skip(1).ToArray());

   public IObject HeadTail => new KTuple(Head, Tail);

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);
}