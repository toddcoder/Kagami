using System.Collections.Generic;
using System.Linq;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using Standard.Types.Monads;
using Standard.Types.Numbers;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Objects
{
   public class Dictionary : IObject, IMutableCollection
   {
      public static IObject New(IObject defaultValue, Boolean caching)
      {
         var dictionary = new Dictionary();
         if (defaultValue is Lambda lambda)
            dictionary.DefaultLambda = lambda.Some();
         else
            dictionary.DefaultValue = defaultValue.Some();
         dictionary.Caching = caching;

         return dictionary;
      }

      public static IObject Empty => new Dictionary(new IObject[0]);

      Hash<IObject, IObject> dictionary;
      IObject[] keys;
      IMaybe<Lambda> defaultLambda;
      int parameterCount;

      public Dictionary(IEnumerable<IObject> items)
      {
         dictionary = new Hash<IObject, IObject>();

         foreach (var item in items)
            if (item is IKeyValue kv)
               dictionary[kv.Key] = kv.Value;

         keys = new IObject[0];
         parameterCount = 0;
         defaultLambda = none<Lambda>();
      }

      public Dictionary() : this(new IObject[0]) { }

      public Dictionary(Hash<IObject, IObject> hash)
      {
         dictionary = hash;
         keys = new IObject[0];
         parameterCount = 0;
         defaultLambda = none<Lambda>();
      }

      public IMaybe<IObject> DefaultValue { get; set; } = none<IObject>();

      public IMaybe<Lambda> DefaultLambda
      {
         get => defaultLambda;
         set
         {
            defaultLambda = value;
            parameterCount = defaultLambda.FlatMap(l => l.Invokable.Parameters.Length, () => 0);
         }

      }

      public Boolean Caching { get; set; } = false;

      IObject getValue(IObject key)
      {
         if (dictionary.ContainsKey(key))
            return dictionary[key];
         else if (DefaultValue.If(out var dv))
         {
            if (Caching.IsTrue)
               dictionary[key] = dv;
            return dv;
         }
         else if (defaultLambda.If(out var lambda))
         {
            IObject value;
            switch (parameterCount)
            {
               case 1:
                  value = lambda.Invoke(key);
                  break;
               case 2:
                  value = lambda.Invoke(this, key);
                  break;
               default:
                  return Unassigned.Value;
            }
            if (Caching.IsTrue)
               dictionary[key] = value;
            return value;
         }
         else
            return Unassigned.Value;
      }

      public IObject this[IObject key]
      {
         get => getValue(key);
         set
         {
            if (value is Del)
               dictionary.Remove(key);
            else
               dictionary[key] = value;
         }
      }

      public IObject Get(IObject key) => dictionary.FlatMap(key, v => new Some(v), () => Nil.NilValue);

      public string ClassName => "Dictionary";

      public string AsString => $"{{{dictionary.Select(i => $"{i.Key.AsString} => {i.Value.AsString}").Listify(" ")}}}";

      public string Image =>
         dictionary.Count == 0 ? "{:}" : $"{{{dictionary.Select(i => $"{i.Key.Image} => {i.Value.Image}").Listify()}}}";

      public int Hash => dictionary.GetHashCode();

      public bool IsEqualTo(IObject obj)
      {
         return obj is Dictionary dict && dictionary.Count == dict.dictionary.Count && dictionary
            .Select(i => dict.dictionary.ContainsKey(i.Key) && dict.dictionary[i.Key].IsEqualTo(i.Value)).All(b => b);
      }

      public bool Match(IObject comparisand, Hash<string, IObject> bindings)
      {
         return match(this, comparisand, (d1, d2) =>
         {
            var di1 = d1.dictionary;
            var di2 = d2.dictionary;
            return di1.Count == di2.Count && di1.Select(i => di2.ContainsKey(i.Key) && i.Value.Match(d2[i.Key], bindings)).All(b => b);
         }, bindings);
      }

      public bool IsTrue => dictionary.Count > 0;

      public IIterator GetIterator(bool lazy) => lazy ? (IIterator)new LazyDictionaryIterator(this) : new DictionaryIterator(this);

      public IMaybe<IObject> Next(int index) => when(keys.Length < index, () => dictionary[keys[index]]);

      public IMaybe<IObject> Peek(int index) => when(keys.Length < index, () => dictionary[keys[index]]);

      public Int Length => dictionary.Count;

      public IEnumerable<IObject> List => dictionary.Select(i => (IObject)new Tuple(i.Key, i.Value));

      public bool ExpandForArray => false;

      public IObject Delete(IObject key)
      {
         if (dictionary.ContainsKey(key))
         {
            var value = dictionary[key];
            dictionary.Remove(key);
            return new Some(value);
         }
         else
            return Nil.NilValue;
      }

      public IObject Keys => new Array(dictionary.KeyArray());

      public IObject Values => new Array(dictionary.ValueArray());

      public Boolean In(IObject key) => dictionary.ContainsKey(key);

      public Boolean NotIn(IObject key) => !dictionary.ContainsKey(key);

      public IObject Times(int count) => this;

      public IIterator GetIndexedIterator() => new IndexedIterator(this);

      public IObject Swap(IObject key1, IObject key2)
      {
         var value1 = getValue(key1);
         var value2 = getValue(key2);
         this[key1] = value2;
         this[key2] = value1;

         return this;
      }

      public IObject Clear()
      {
         dictionary.Clear();
         return this;
      }

      public Hash<IObject, IObject> InternalHash => dictionary;

      public IObject Update(IObject key, IObject value)
      {
         if (dictionary.ContainsKey(key))
         {
            var oldValue = dictionary[key];
            dictionary[key] = value;

            return new Some(oldValue);
         }
         else
         {
            dictionary[key] = value;
            return Nil.NilValue;
         }
      }

      public IObject Append(IObject obj)
      {
         if (obj is Tuple t)
         {
            var key = t[0];
            var value = t[1];
            this[key] = value;

            return this;
         }
         else
            return this;
      }

      public IObject Remove(IObject obj)
      {
         if (dictionary.ContainsKey(obj))
         {
            var oldValue = dictionary[obj];
            dictionary.Remove(obj);

            return new Some(oldValue);
         }
         else
         {
            dictionary.Remove(obj);
            return Nil.NilValue;
         }
      }

      public IObject RemoveAt(int index)
      {
         var keyArray = dictionary.KeyArray();
         if (index.Between(0).Until(keyArray.Length))
            return Remove(keyArray[index]);
         else
            return Nil.NilValue;
      }

      public IObject InsertAt(int index, IObject obj)
      {
         var keyArray = dictionary.KeyArray();
         if (index.Between(0).Until(keyArray.Length))
            return Update(keyArray[index], obj);
         else
            return Nil.NilValue;
      }

      public IObject[] KeyArray => dictionary.KeyArray();

      public Dictionary Merge(Dictionary other)
      {
         foreach (var (key, value) in other.InternalHash)
            this[key] = value;

         return this;
      }

	   public Dictionary ForEach(Lambda lambda)
	   {
		   var keyArray = KeyArray;
		   foreach (var key in keyArray)
			   dictionary[key] = lambda.Invoke(key, dictionary[key]);

         return this;
	   }
   }
}