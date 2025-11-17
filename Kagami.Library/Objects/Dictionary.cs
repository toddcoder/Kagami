using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Operations.OperationFunctions;

namespace Kagami.Library.Objects;

public class Dictionary : IObject, IMutableCollection, IMutable
{
   public static IObject New(IObject defaultValue, KBoolean caching, Maybe<TypeConstraint> _typeConstraint)
   {
      var dictionary = new Dictionary() { TypeConstraint = _typeConstraint };
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

   public static Dictionary Empty => new Dictionary(Enumerable.Empty<IObject>());

   public static IObject New(IEnumerable<IObject> objects, Maybe<TypeConstraint> _typeConstraint)
   {
      return new Dictionary(objects) { TypeConstraint = _typeConstraint };
   }

   protected Hash<IObject, IObject> dictionary = [];
   protected IObject[] keys = [];
   protected Maybe<TypeConstraint> _keyTypeConstraint = nil;
   protected Maybe<TypeConstraint> _valueTypeConstraint = nil;

   protected void assertIncomingKeyIsEquivalent(IObject key)
   {
      if (_keyTypeConstraint is (true, var keyTypeConstraint))
      {
         if (!keyTypeConstraint.Matches(classOf(key)))
         {
            throw fail($"Key {key.AsString} is incompatible with {keyTypeConstraint.AsString}");
         }
      }
   }

   protected void assertIncomingValueIsEquivalent(IObject value)
   {
      if (_valueTypeConstraint is (true, var valueTypeConstraint))
      {
         if (!valueTypeConstraint.Matches(classOf(value)))
         {
            throw fail($"Value {value.AsString} is incompatible with {valueTypeConstraint.AsString}");
         }
      }
   }

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
                     DefaultLambda = lambda;
                     break;
                  default:
                     DefaultValue = kv.Value.Some();
                     break;
               }

               break;
            case IKeyValue kv:
               if (kv.Key is IMutable)
               {
                  throw dictionaryKeyMustBeImmutable();
               }

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

   public Maybe<Lambda> DefaultLambda { get; set; } = nil;

   public KBoolean Caching { get; set; }

   protected IObject getValue(IObject key)
   {
      if (dictionary.Maybe[key] is (true, var value))
      {
         if (DefaultValue || DefaultLambda)
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
      else if (DefaultLambda is (true, var lambda))
      {
         switch (lambda.ParameterCount.Value)
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
         return KNil.NilValue;
      }
   }

   public IObject this[IObject key]
   {
      get
      {
         assertIncomingKeyIsEquivalent(key);
         return getValue(key);
      }
      set
      {
         assertIncomingKeyIsEquivalent(key);
         assertIncomingValueIsEquivalent(value);
         switch (value)
         {
            case Dictionary otherDictionary when Id == otherDictionary.Id:
               return;
            case KNil:
               dictionary.Remove(key);
               break;
            default:
               if (key is IMutable)
               {
                  throw dictionaryKeyMustBeImmutable();
               }

               dictionary[key] = value;
               break;
         }
      }
   }

   public IObject this[Sequence sequence]
   {
      get
      {
         Hash<IObject, IObject> hash = [];
         foreach (var key in sequence.List)
         {
            if (dictionary.Maybe[key] is (true, var value))
            {
               hash[key] = value;
            }
         }

         return new Dictionary(hash);
      }
      set
      {
         assertIncomingValueIsEquivalent(value);
         switch (value)
         {
            case Dictionary otherDictionary when Id == otherDictionary.Id:
               return;
            case KNil:
            {
               foreach (var key in sequence.List)
               {
                  assertIncomingKeyIsEquivalent(key);
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
                  foreach (var key in sequence.List)
                  {
                     assertIncomingKeyIsEquivalent(key);
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
               foreach (var key in sequence.List)
               {
                  assertIncomingKeyIsEquivalent(key);
                  this[key] = value;
               }
            }
               break;
         }
      }
   }

   public IObject GetRaw(IObject key) => dictionary[key];

   public string ClassName => "Dictionary";

   public string AsString
   {
      get
      {
         if (dictionary.Count == 0)
         {
            return "";
         }
         else
         {
            return $"{dictionary.Select(i => $"{i.Key.AsString} : {i.Value.AsString}").ToString(" ")}";
         }
      }
   }

   public string Image
   {
      get
      {
         var image = dictionary.Count == 0 ? "{:}" : $"{{{dictionary.Select(i => $"{i.Key.Image} : {i.Value.Image}").ToString(", ")}}}";
         return image + (DefaultLambda.Map(l => l.Image) | "") + (TypeConstraint.Map(tc => $" {tc.Image}") | "");
      }
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

   public Guid Id { get; init; } = Guid.NewGuid();

   public IIterator GetIterator(bool lazy) => lazy ? new LazyDictionaryIterator(this) : new DictionaryIterator(this);

   public Maybe<IObject> Next(int index) => maybe<IObject>() & keys.Length < index & (() => dictionary[keys[index]]);

   public Maybe<IObject> Peek(int index) => maybe<IObject>() & keys.Length < index & (() => dictionary[keys[index]]);

   public Int Length => dictionary.Count;

   public IEnumerable<IObject> List => dictionary.Select(i => (IObject)new KTuple(i.Key, i.Value));

   public bool ExpandForArray => false;

   public IObject Delete(IObject key)
   {
      assertIncomingKeyIsEquivalent(key);
      if (dictionary.Maybe[key] is (true, var value))
      {
         dictionary.Remove(key);
         return new Some(value);
      }
      else
      {
         return KNil.NilValue;
      }
   }

   public IObject Keys => new Set(dictionary.KeyArray()) { TypeConstraint = _keyTypeConstraint };

   public IObject Values => new KArray(dictionary.ValueArray()) { TypeConstraint = _keyTypeConstraint };

   public KBoolean In(IObject key)
   {
      assertIncomingKeyIsEquivalent(key);
      return dictionary.ContainsKey(key);
   }

   public KBoolean NotIn(IObject key)
   {
      assertIncomingKeyIsEquivalent(key);
      return !dictionary.ContainsKey(key);
   }

   public IObject Times(int count) => this;

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IObject One()
   {
      if (dictionary.Count == 1)
      {
         var first = dictionary.First();
         return new KTuple(first.Key, first.Value);
      }
      else
      {
         return this;
      }
   }

   public IObject Copy()
   {
      Hash<IObject, IObject> hash = [];
      foreach (var (key, value) in dictionary)
      {
         hash[key] = value;
      }

      return new Dictionary(hash) { TypeConstraint = TypeConstraint };
   }

   public IIterator Following(IObject following) => new MultiIterator(this, following);

   public Maybe<TypeConstraint> TypeConstraint
   {
      get;
      set
      {
         if (value is (true, var typeConstraint))
         {
            var comparisands = typeConstraint.Comparisands;
            var count = comparisands.Length;
            switch (count)
            {
               case 1:
                  _keyTypeConstraint = new TypeConstraint([comparisands[0]]);
                  _valueTypeConstraint = new TypeConstraint([comparisands[0]]);
                  break;
               case 2:
                  _keyTypeConstraint = new TypeConstraint([comparisands[0]]);
                  _valueTypeConstraint = new TypeConstraint([comparisands[1]]);
                  break;
               default:
                  throw fail("One or two comparisands allowed");
            }

            foreach (var (key, dictionaryValue) in dictionary)
            {
               assertIncomingKeyIsEquivalent(key);
               assertIncomingValueIsEquivalent(dictionaryValue);
            }
         }

         field = value;
      }
   } = nil;

   public IObject Swap(IObject key1, IObject key2)
   {
      assertIncomingKeyIsEquivalent(key1);
      assertIncomingKeyIsEquivalent(key2);
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
      assertIncomingKeyIsEquivalent(key);
      assertIncomingValueIsEquivalent(value);

      if (key is IMutable)
      {
         throw dictionaryKeyMustBeImmutable();
      }

      if (dictionary.Maybe[key] is (true, var oldValue))
      {
         dictionary[key] = value;
         return new Some(oldValue);
      }
      else
      {
         dictionary[key] = value;
         return KNil.NilValue;
      }
   }

   public IObject Append(IObject obj)
   {
      if (obj is KTuple t)
      {
         var key = t[0];
         assertIncomingKeyIsEquivalent(key);
         var value = t[1];
         assertIncomingValueIsEquivalent(value);
         this[key] = value;
      }

      return this;
   }

   public IObject Remove(IObject obj)
   {
      assertIncomingKeyIsEquivalent(obj);
      if (dictionary.Maybe[obj] is (true, var oldValue))
      {
         dictionary.Remove(obj);
         return new Some(oldValue);
      }
      else
      {
         dictionary.Remove(obj);
         return KNil.NilValue;
      }
   }

   public IObject RemoveAt(int index)
   {
      var keyArray = dictionary.KeyArray();
      return index.Between(0).Until(keyArray.Length) ? Remove(keyArray[index]) : KNil.NilValue;
   }

   public IObject RemoveAll(IObject obj) => Remove(obj);

   public IObject RemoveKeys(IObject keys)
   {
      switch (keys)
      {
         case ICollection collection:
         {
            var iterator = collection.GetIterator(false);
            return removeKeys(iterator);
         }
         case IIterator iterator:
         {
            return removeKeys(iterator);
         }
         default:
            return Remove(keys);
      }

      IObject removeKeys(IIterator iterator)
      {
         Hash<IObject, IObject> removed = [];
         foreach (var key in iterator.List())
         {
            assertIncomingKeyIsEquivalent(key);
            var _value = dictionary.Maybe[key];
            if (_value is (true, var value))
            {
               removed[key] = value;
               dictionary.Remove(key);
            }
         }

         return new Dictionary(removed);
      }
   }

   public IObject InsertAt(int index, IObject obj)
   {
      assertIncomingValueIsEquivalent(obj);
      var keyArray = dictionary.KeyArray();
      return index.Between(0).Until(keyArray.Length) ? Update(keyArray[index], obj) : KNil.NilValue;
   }

   public KBoolean IsEmpty => dictionary.Count == 0;

   public KBoolean IsNotEmpty => dictionary.Count > 0;

   public IObject Assign(SkipTake skipTake, IEnumerable<IObject> values) => this;

   public IObject Prepend(IObject obj)
   {
      switch (obj)
      {
         case KTuple t:
         {
            var key = t[0];
            assertIncomingKeyIsEquivalent(key);
            var value = t[1];
            assertIncomingValueIsEquivalent(value);
            this[key] = value;
            break;
         }
         case NameValue nameValue:
         {
            var key = (KString)nameValue.Name;
            assertIncomingKeyIsEquivalent(key);
            var value = nameValue.Value;
            assertIncomingValueIsEquivalent(value);
            this[key] = value;
            break;
         }
         case IKeyValue kv:
            assertIncomingKeyIsEquivalent(kv.Key);
            assertIncomingValueIsEquivalent(kv.Value);
            this[kv.Key] = kv.Value;
            break;
      }

      return this;
   }

   public IObject[] KeyArray => dictionary.KeyArray();

   public Dictionary Merge(Dictionary other)
   {
      Hash<IObject, IObject> hash = [];
      foreach (var (key, value) in dictionary)
      {
         hash[key] = value;
      }

      foreach (var (key, value) in other.InternalHash)
      {
         assertIncomingKeyIsEquivalent(key);
         assertIncomingValueIsEquivalent(value);
         hash[key] = value;
      }

      return new Dictionary(hash);
   }

   public Dictionary Merge(Dictionary other, Lambda lambda)
   {
      Hash<IObject, IObject> hash = [];
      foreach (var (key, value) in dictionary)
      {
         hash[key] = value;
      }

      foreach (var (key, value) in other.InternalHash)
      {
         assertIncomingKeyIsEquivalent(key);
         assertIncomingValueIsEquivalent(value);
         if (hash.Maybe[key] is (true, var existingValue))
         {
            hash[key] = lambda.Invoke(key, existingValue, value);
         }
         else
         {
            hash[key] = value;
         }
      }

      return new Dictionary(hash);
   }

   public Dictionary ForEach(Lambda lambda)
   {
      var keyArray = KeyArray;
      foreach (var key in keyArray)
      {
         if (key is IMutable)
         {
            throw dictionaryKeyMustBeImmutable();
         }

         var result = lambda.Invoke(key, dictionary[key]);
         assertIncomingValueIsEquivalent(result);
         dictionary[key] = result;
      }

      return this;
   }

   public Dictionary Invert(bool alwaysArray)
   {
      var memo = new Memo<IObject, List<IObject>>.Function(_ => []);
      foreach (var (key, value) in dictionary)
      {
         if (value is IMutable)
         {
            throw dictionaryKeyMustBeImmutable();
         }

         memo[value].Add(key);
      }

      Hash<IObject, IObject> newHash = [];
      if (alwaysArray)
      {
         newHash = memo.ToHash(k => k.Key, IObject (v) => new KArray(v.Value));
      }
      else
      {
         foreach (var (key, list) in memo)
         {
            if (key is IMutable)
            {
               throw dictionaryKeyMustBeImmutable();
            }

            if (list.Count == 1)
            {
               newHash[key] = list[0];
            }
            else
            {
               newHash[key] = new KArray(list);
            }
         }
      }

      return new Dictionary(newHash);
   }

   public Dictionary Concatenate(ICollection collection)
   {
      Hash<IObject, IObject> newDictionary = [];
      foreach (var (key, value) in dictionary)
      {
         if (key is IMutable)
         {
            throw dictionaryKeyMustBeImmutable();
         }

         newDictionary[key] = value;
      }

      var iterator = collection.GetIterator(false);
      foreach (var item in iterator.List())
      {
         if (item is KTuple { Length.Value: 2 } tuple)
         {
            if (tuple[0] is IMutable)
            {
               throw dictionaryKeyMustBeImmutable();
            }

            assertIncomingKeyIsEquivalent(tuple[0]);
            assertIncomingValueIsEquivalent(tuple[1]);
            newDictionary[tuple[0]] = tuple[1];
         }
      }

      return new Dictionary(newDictionary);
   }

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);

   public IObject Items
   {
      get
      {
         List<IObject> items = [];
         foreach (var (key, value) in dictionary)
         {
            var tuple = new KTuple(("key", key), ("value", value));
            items.Add(tuple);
         }

         return new KArray([.. items]);
      }
   }

   public Dictionary Memo(Lambda lambda)
   {
      DefaultLambda = lambda;
      DefaultValue = nil;
      Caching = true;

      return this;
   }

   public Dictionary UpdateIfNil(IObject key, IObject value)
   {
      assertIncomingKeyIsEquivalent(key);
      assertIncomingValueIsEquivalent(value);
      if (!dictionary.ContainsKey(key))
      {
         if (key is IMutable)
         {
            throw dictionaryKeyMustBeImmutable();
         }

         dictionary[key] = value;
      }

      return this;
   }

   public IObject GetValue(IObject key)
   {
      var obj = getValue(key);
      return obj switch
      {
         KNil => throw fail($"Key {key} not found"),
         Some some => some.Value,
         _ => obj
      };
   }
}