using Kagami.Library.Objects;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class DictionaryClass : BaseClass, ICollectionClass
{
   public override string Name => "Dictionary";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      collectionMessages();
      mutableCollectionMessages();
      typedCollectionMessages();

      messages["[](_)"] = (obj, msg) => function<Dictionary>(obj, d => getKeyed(d, msg.Arguments[0]));
      messages["[]=(_,_)"] = (obj, msg) => function<Dictionary>(obj, d => setKeyed(d, msg.Arguments[0], msg.Arguments[1]));
      messages["default".get()] = (obj, _) => function<Dictionary>(obj, d =>
      {
         if (d.DefaultValue is (true, var defaultValue))
         {
            return defaultValue;
         }
         else if (d.DefaultLambda is (true, var defaultLambda))
         {
            return defaultLambda;
         }
         else
         {
            throw fail("Default is not assigned");
         }
      });
      messages["default".set()] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, v) =>
      {
         if (v is Lambda lambda)
         {
            d.DefaultLambda = lambda.Some();
         }
         else
         {
            d.DefaultValue = v.Some();
         }

         return KVoid.Value;
      });
      messages["caching".get()] = (obj, _) => function<Dictionary>(obj, d => d.Caching);
      messages["caching".set()] = (obj, msg) => function<Dictionary, KBoolean>(obj, msg, (d, b) => d.Caching = b);
      messages[">>(_)"] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, k) => d.Delete(k));
      messages["keys".get()] = (obj, _) => function<Dictionary>(obj, d => d.Keys);
      messages["values".get()] = (obj, _) => function<Dictionary>(obj, d => d.Values);
      messages["items".get()] = (obj, _) => function<Dictionary>(obj, d => d.Items);
      messages["in(_)"] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, k) => d.In(k));
      messages["notIn(_)"] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, k) => d.NotIn(k));
      messages["swap(_,_)"] = (obj, msg) => function<Dictionary, IObject, IObject>(obj, msg, (d, k1, k2) => d.Swap(k1, k2));
      messages["clear()"] = (obj, _) => function<Dictionary>(obj, d => d.Clear());
      messages["update(key:_,value:_)"] =
         (obj, msg) => function<Dictionary, IObject, IObject>(obj, msg, (d, k, v) => d.Update(k, v));
      messages["merge(_<Dictionary>)"] = (obj, msg) => function<Dictionary, Dictionary>(obj, msg, (d1, d2) => d1.Merge(d2));
      messages["merge(_<Dictionary>,_<Lambda>)"] = (obj, msg) => function<Dictionary, Dictionary, Lambda>(obj, msg, (d1, d2, l) => d1.Merge(d2, l));
      messages["remove(key:_)"] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, k) => d.Remove(k));
      messages["remove(keys:_)"] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, k) => d.RemoveKeys(k));
      messages["forEach(_<Lambda>)"] = (obj, msg) => function<Dictionary, Lambda>(obj, msg, (d, l) => d.ForEach(l));
      messages["invert(alwaysArray:_<Boolean>)"] = (obj, msg) => function<Dictionary, KBoolean>(obj, msg, (d, a) => d.Invert(a.Value));
      messages["invert()"] = (obj, _) => function<Dictionary>(obj, d => d.Invert(true));
      messages["~(_)"] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, o) => d.Concatenate((ICollection)o));
      messages["memo(_)"] = (obj, msg) => function<Dictionary, Lambda>(obj, msg, (d, l) => d.Memo(l));
      messages["updateIfNil(key:_,value:_)"] =
         (obj, msg) => function<Dictionary, IObject, IObject>(obj, msg, (d, k, v) => d.UpdateIfNil(k, v));
      messages["skip(_)"] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, l) => d.GetValue(l));
   }

   protected static IObject getKeyed(Dictionary dictionary, IObject key) => key switch
   {
      Sequence internalList => dictionary[internalList],
      ICollection collection and not KString => dictionary[new Sequence(collection.GetIterator(false).List())],
      IIterator iterator => dictionary[new Sequence(iterator.List())],
      _ => dictionary[key]
   };

   protected static IObject setKeyed(Dictionary dictionary, IObject key, IObject value)
   {
      switch (key)
      {
         case Sequence internalList:
            dictionary[internalList] = value;
            return dictionary;
         case ICollection collection and not KString:
            dictionary[new Sequence(collection.GetIterator(false).List())] = value;
            return dictionary;
         case IIterator iterator:
            dictionary[new Sequence(iterator.List())] = value;
            return dictionary;
         default:
            dictionary[key] = value;
            return dictionary;
      }
   }

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      classMessages["new(default:_,caching:_<Boolean>)"] = (cls, msg) =>
         classFunc<DictionaryClass, IObject, KBoolean>(cls, msg, (_, d, c) => Dictionary.New(d, c, nil));
      classMessages["new(default:_)"] =
         (cls, msg) => classFunc<DictionaryClass, IObject>(cls, msg, (_, d) => Dictionary.New(d, false, nil));
      classMessages["empty".get()] = (cls, _) => classFunc<DictionaryClass>(cls, _ => Dictionary.Empty);
   }

   public override IObject DefaultValue => Dictionary.Empty;

   public TypeConstraint EquivalentTypeConstraint() => TypeConstraint.FromList("Collection");

   public IObject Revert(IEnumerable<IObject> list, Maybe<TypeConstraint> _typeConstraint) => new Dictionary(list);
}