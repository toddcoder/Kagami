using System;
using System.Collections.Generic;
using System.Linq;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using Standard.Types.Monads;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Arrays.ArrayFunctions;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Objects
{
   public struct Tuple : IObject, IEquatable<Tuple>, ICollection
   {
      public static IObject NewTuple(IObject x, IObject y)
      {
         if (x is Tuple t)
            return new Tuple(t, y);
         else
            return new Tuple(x, y);
      }

      public static IObject NewTupleNamed(string nameX, IObject x, string nameY, IObject y)
      {
         return new Tuple(array((IObject)new KeyValue(nameX, x), new KeyValue(nameY, y)));
      }

      public static Tuple Empty => new Tuple(new IObject[0]);

      public static Tuple Tuple3(string left, string middle, string right)
      {
         return new Tuple(new[] { String.StringObject(left), String.StringObject(middle), String.StringObject(right) });
      }

      IObject[] items;
      Hash<string, int> names;
      Hash<int, string> indexes;

      public Tuple(IObject[] items) : this()
      {
         if (items.Length == 1 && items[0] is InternalList il && il.ExpandInTuple)
            this.items = il.List.ToArray();
         else
            this.items = items;
         names = new Hash<string, int>();
         indexes = new Hash<int, string>();

         denameify();
      }

      public Tuple(IObject x, IObject y)
      {
         items = array(x, y);
         names = new Hash<string, int>();
         indexes = new Hash<int, string>();

         denameify();
      }

      public Tuple(IObject value) : this(array(value)) { }

      void denameify()
      {
         for (var i = 0; i < items.Length; i++)
            if (items[i] is IKeyValue keyValue && keyValue.ExpandInTuple)
            {
               names[keyValue.Key.AsString] = i;
               indexes[i] = keyValue.Key.AsString;
               items[i] = keyValue.Value;
            }
      }

      public Tuple(Tuple tuple, IObject item) : this()
      {
         var tupleItems = tuple.items;
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
               return items[names[name]];
            else
               throw keyNotFound((String)name);
         }
      }

      string getItemString(int index, string text)
      {
         return indexes.FlatMap(index, n => $"{n}: {text}", () => text);
      }

      string getItemString(int index) => getItemString(index, items[index].AsString);

      string getItemImage(int index) => getItemString(index, items[index].Image);

      IMaybe<IObject[]> AllButLast
      {
         get
         {
            var self = this;
            return when(items.Length > 0, () => self.items.Take(self.items.Length - 1).ToArray());
         }
      }

      public IMaybe<IObject> Last
      {
         get
         {
            var self = this;
            return when(items.Length > 0, () => self.items[self.items.Length - 1]);
         }
      }

      public bool ContainsName(string name) => names.ContainsKey(name);

      public string ClassName => "Tuple";

      public string AsString
      {
         get
         {
            var self = this;
            return items.Select((o, i) => self.getItemString(i)).Listify(" ");
         }
      }

      public string Image
      {
         get
         {
            var self = this;
            return $"({items.Select((o, i) => self.getItemImage(i)).Listify()})";
         }
      }

      public int Hash => items.GetHashCode();

      public bool IsEqualTo(IObject obj)
      {
         return obj is Tuple t && items.Length == t.items.Length &&
            Enumerable.Zip(items, t.items, (t1, t2) => (x: t1, y: t2)).All(tu => tu.x.IsEqualTo(tu.y));
      }

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, (t1, t2) =>
      {
         if (t1.Length.Value != t2.Length.Value)
            return false;
         else
            return Enumerable.Zip(t1.items, t2.items, (i1, i2) => i1.Match(i2, bindings)).All(b => b);
      }, bindings);

      public bool IsTrue => items.Length > 0;

      public bool Equals(Tuple other) => IsEqualTo(other);

      public override bool Equals(object obj) => obj is Tuple tuple && Equals(tuple);

      public override int GetHashCode() => Hash;

      //public string FullFunctionName(string name) => name.Function(names.KeyArray());

      public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

      public IMaybe<IObject> Next(int index)
      {
         var self = this;
         return when(index < items.Length, () => self.items[index]);
      }

      public IMaybe<IObject> Peek(int index)
      {
         var self = this;
         return when(index < items.Length, () => self.items[index]);
      }

      public Int Length => items.Length;

      public IEnumerable<IObject> List
      {
         get
         {
            foreach (var item in items)
               yield return item;
         }
      }

      public bool ExpandForArray => false;

      public Boolean In(IObject item) => items.Contains(item);

      public Boolean NotIn(IObject item) => !items.Contains(item);

      public IObject Times(int count)
      {
         var result = new List<IObject>();
         for (var i = 0; i < count; i++)
            result.AddRange(items);

         return new Tuple(result.ToArray());
      }

      public IIterator GetIndexedIterator() => new IndexedIterator(this);
   }
}