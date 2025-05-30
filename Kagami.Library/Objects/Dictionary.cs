using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Operations.OperationFunctions;

namespace Kagami.Library.Objects;

public class Dictionary : IObject, IMutableCollection
{
   public static IObject New(IObject defaultValue, KBoolean caching)
   {
      var dictionary = new Dictionary();
      if (defaultValue is Lambda lambda)
      {
         dictionary.DefaultLambda = lambda.Some();
      }
      else
      {
         dictionary.DefaultValue = defaultValue.Some();
      }

      dictionary.Caching = caching;

      return dictionary;
   }

   public static IObject Empty => new Dictionary(Enumerable.Empty<IObject>());

   public static IObject New(IEnumerable<IObject> objects) => new Dictionary(objects);

   protected int objectID = uniqueObjectID();
   protected Hash<IObject, IObject> dictionary = [];
   protected IObject[] keys = [];
   protected Maybe<Lambda> _defaultLambda = nil;
   protected int parameterCount;

   public Dictionary(IEnumerable<IObject> items)
   {
      foreach (var item in items)
      {
         switch (item)
         {
            case IKeyValue kv when kv.Key.IsEqualTo(Any.Value):

               switch (kv.Value)
               {
                  case Lambda lambda:
                     _defaultLambda = lambda;
                     break;
                  default:
                     DefaultValue = kv.Value.Some();
                     break;
               }

               break;
            case IKeyValue kv:
               dictionary[kv.Key] = kv.Value;
               break;
            case KTuple tuple:
               dictionary[tuple[0]] = tuple[1];
               break;
         }
      }
   }

   public Dictionary() : this(Array.Empty<IObject>())
   {
   }

   public Dictionary(Hash<IObject, IObject> hash)
   {
      dictionary = hash;
   }

   public Maybe<IObject> DefaultValue { get; set; } = nil;

   public Maybe<Lambda> DefaultLambda
   {
      get => _defaultLambda;
      set
      {
         _defaultLambda = value;
         parameterCount = _defaultLambda.Map(l => l.Invokable.Parameters.Length) | (() => 0);
      }
   }

   public KBoolean Caching { get; set; }

   protected IObject getValue(IObject key)
   {
      if (dictionary.Maybe[key] is (true, var value))
      {
         if (DefaultValue || _defaultLambda)
         {
            return value;
         }
         else
         {
            return Some.Object(value);
         }
      }
      else if (DefaultValue is (true, var defaultValue))
      {
         if (Caching.IsTrue)
         {
            dictionary[key] = defaultValue;
         }

         return defaultValue;
      }
      else if (_defaultLambda is (true, var lambda))
      {
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
         {
            dictionary[key] = value;
         }

         return value;
      }
      else
      {
         return None.NoneValue;
      }
   }

   public IObject this[IObject key]
   {
      get => getValue(key);
      set
      {
         switch (value)
         {
            case Dictionary otherDictionary when objectID == otherDictionary.objectID:
               return;
            case None:
               dictionary.Remove(key);
               break;
            default:
               dictionary[key] = value;
               break;
         }
      }
   }

   public IObject this[Container container]
   {
      get
      {
         var list = container.List.Where(key => dictionary.ContainsKey(key)).Select(key => this[key]).ToList();
         return new KArray(list);
      }
      set
      {
         switch (value)
         {
            case Dictionary otherDictionary when objectID == otherDictionary.objectID:
               return;
            case None:
            {
               foreach (var key in container.List)
               {
                  dictionary.Remove(key);
               }
            }
               break;
            case ICollection and not KString:
            case IIterator:
            {
               var _iterator = getIterator(value, false);
               if (_iterator is (true, var iterator))
               {
                  foreach (var key in container.List)
                  {
                     var _item = iterator.Next();
                     if (_item is (true, var item))
                     {
                        this[key] = item;
                     }
                     else
                     {
                        break;
                     }
                  }
               }
               else
               {
                  throw _iterator.Exception;
               }
            }
               break;
            default:
            {
               foreach (var key in container.List)
               {
                  this[key] = value;
               }
            }
               break;
         }
      }
   }

   public IObject Get(IObject key)
   {
      if (dictionary.Maybe[key] is (true, var value))
      {
         if (DefaultValue || DefaultLambda)
         {
            return value;
         }
         else
         {
            return new Some(value);
         }
      }
      else if (_defaultLambda is (true, var lambda))
      {
         var result = lambda.Invoke(key);
         if (Caching.IsTrue)
         {
            dictionary[key] = result;
         }

         return result;
      }
      else if (DefaultValue is (true, var defaultValue))
      {
         if (Caching.IsTrue)
         {
            dictionary[key] = defaultValue;
         }

         return defaultValue;
      }
      else
      {
         return None.NoneValue;
      }
   }

   public IObject GetRaw(IObject key) => dictionary[key];

   public string ClassName => "Dictionary";

   public string AsString => $"{{{dictionary.Select(i => $"{i.Key.AsString} => {i.Value.AsString}").ToString(" ")}}}";

   public string Image
   {
      get => dictionary.Count == 0 ? "{}" : $"{{{dictionary.Select(i => $"{i.Key.Image} => {i.Value.Image}").ToString(", ")}}}";
   }

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

   public IIterator GetIterator(bool lazy) => lazy ? new LazyDictionaryIterator(this) : new DictionaryIterator(this);

   public Maybe<IObject> Next(int index) => maybe<IObject>() & keys.Length < index & (() => dictionary[keys[index]]);

   public Maybe<IObject> Peek(int index) => maybe<IObject>() & keys.Length < index & (() => dictionary[keys[index]]);

   public Int Length => dictionary.Count;

   public IEnumerable<IObject> List => dictionary.Select(i => (IObject)new KTuple(i.Key, i.Value));

   public bool ExpandForArray => false;

   public IObject Delete(IObject key)
   {
      if (dictionary.Maybe[key] is (true, var value))
      {
         dictionary.Remove(key);
         return new Some(value);
      }
      else
      {
         return None.NoneValue;
      }
   }

   public IObject Keys => new Set(dictionary.KeyArray());

   public IObject Values => new KArray(dictionary.ValueArray());

   public KBoolean In(IObject key) => dictionary.ContainsKey(key);

   public KBoolean NotIn(IObject key) => !dictionary.ContainsKey(key);

   public IObject Times(int count) => this;

   public KString MakeString(string connector) => makeString(this, connector);

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
      if (dictionary.Maybe[key] is (true, var oldValue))
      {
         dictionary[key] = value;
         return new Some(oldValue);
      }
      else
      {
         dictionary[key] = value;
         return None.NoneValue;
      }
   }

   public IObject Append(IObject obj)
   {
      if (obj is KTuple t)
      {
         var key = t[0];
         var value = t[1];
         this[key] = value;

         return this;
      }
      else
      {
         return this;
      }
   }

   public IObject Remove(IObject obj)
   {
      if (dictionary.Maybe[obj] is (true, var oldValue))
      {
         dictionary.Remove(obj);
         return new Some(oldValue);
      }
      else
      {
         dictionary.Remove(obj);
         return None.NoneValue;
      }
   }

   public IObject RemoveAt(int index)
   {
      var keyArray = dictionary.KeyArray();
      return index.Between(0).Until(keyArray.Length) ? Remove(keyArray[index]) : None.NoneValue;
   }

   public IObject RemoveAll(IObject obj) => Remove(obj);

   public IObject InsertAt(int index, IObject obj)
   {
      var keyArray = dictionary.KeyArray();
      return index.Between(0).Until(keyArray.Length) ? Update(keyArray[index], obj) : None.NoneValue;
   }

   public KBoolean IsEmpty => dictionary.Count == 0;

   public IObject Assign(SkipTake skipTake, IEnumerable<IObject> values) => this;

   public IObject[] KeyArray => dictionary.KeyArray();

   public Dictionary Merge(Dictionary other)
   {
      foreach (var (key, value) in other.InternalHash)
      {
         this[key] = value;
      }

      return this;
   }

   public Dictionary ForEach(Lambda lambda)
   {
      var keyArray = KeyArray;
      foreach (var key in keyArray)
      {
         dictionary[key] = lambda.Invoke(key, dictionary[key]);
      }

      return this;
   }

   public Dictionary Invert()
   {
      var newDictionary = new Hash<IObject, IObject>();
      foreach (var (key, value) in dictionary)
      {
         newDictionary[value] = key;
      }

      return new Dictionary(newDictionary);
   }

   public Dictionary Concatenate(ICollection collection)
   {
      var newDictionary = new Hash<IObject, IObject>();
      foreach (var (key, value) in dictionary)
      {
         newDictionary[key] = value;
      }

      var iterator = collection.GetIterator(false);
      foreach (var item in iterator.List())
      {
         if (item is KTuple { Length: { Value: 2 } } tuple)
         {
            newDictionary[tuple[0]] = tuple[1];
         }
      }

      return new Dictionary(newDictionary);
   }

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);
}