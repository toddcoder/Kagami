using System.Collections.Generic;
using Kagami.Library.Objects;
using Core.Monads;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
	public class DictionaryClass : BaseClass, ICollectionClass
	{
		public override string Name => "Dictionary";

		public override void RegisterMessages()
		{
			base.RegisterMessages();

			collectionMessages();
			mutableCollectionMessages();

			messages["[](_)"] = (obj, msg) => function<Dictionary>(obj, d => getKeyed(d, msg.Arguments[0]));
			messages["[]=(_,_)"] = (obj, msg) => function<Dictionary>(obj, d => setKeyed(d, msg.Arguments[0], msg.Arguments[1]));
			messages["default".get()] = (obj, msg) => function<Dictionary>(obj, d =>
			{
				if (d.DefaultValue.If(out var dv))
				{
					return dv;
				}
				else if (d.DefaultLambda.If(out var dl))
				{
					return dl;
				}
				else
				{
					return Unassigned.Value;
				}
			});
			messages["default".set()] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, v) =>
			{
				if (v is Lambda lambda)
				{
					d.DefaultLambda = lambda.Some();
				}

				d.DefaultValue = v.Some();

				return Void.Value;
			});
			messages["caching".get()] = (obj, msg) => function<Dictionary>(obj, d => d.Caching);
			messages["caching".set()] = (obj, msg) => function<Dictionary, Boolean>(obj, msg, (d, b) => d.Caching = b);
			messages[">>(_)"] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, k) => d.Delete(k));
			messages["keys".get()] = (obj, msg) => function<Dictionary>(obj, d => d.Keys);
			messages["values".get()] = (obj, msg) => function<Dictionary>(obj, d => d.Values);
			messages["in(_)"] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, k) => d.In(k));
			messages["notIn(_)"] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, k) => d.NotIn(k));
			messages["swap(_,_)"] = (obj, msg) => function<Dictionary, IObject, IObject>(obj, msg, (d, k1, k2) => d.Swap(k1, k2));
			messages["clear()"] = (obj, msg) => function<Dictionary>(obj, d => d.Clear());
			messages["update(key:_,value:_)"] =
				(obj, msg) => function<Dictionary, IObject, IObject>(obj, msg, (d, k, v) => d.Update(k, v));
			messages["merge(_)"] = (obj, msg) => function<Dictionary, Dictionary>(obj, msg, (d1, d2) => d1.Merge(d2));
			messages["remove(at:_)"] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, k) => d.Remove(k));
			messages["forEach(_<Lambda>)"] = (obj, msg) => function<Dictionary, Lambda>(obj, msg, (d, l) => d.ForEach(l));
			messages["invert()"] = (obj, msg) => function<Dictionary>(obj, d => d.Invert());
			messages["~(_)"] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, o) => d.Concatenate((ICollection)o));
		}

		static IObject getKeyed(Dictionary dictionary, IObject key)
		{
			switch (key)
			{
				case Container internalList:
					return dictionary[internalList];
				case ICollection collection when !(key is String):
					return dictionary[new Container(collection.GetIterator(false).List())];
				case IIterator iterator:
					return dictionary[new Container(iterator.List())];
				default:
					return dictionary[key];
			}
		}

		static IObject setKeyed(Dictionary dictionary, IObject key, IObject value)
		{
			switch (key)
			{
				case Container internalList:
					dictionary[internalList] = value;
					return dictionary;
				case ICollection collection when !(key is String):
					dictionary[new Container(collection.GetIterator(false).List())] = value;
					return dictionary;
				case IIterator iterator:
					dictionary[new Container(iterator.List())] = value;
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
				classFunc<DictionaryClass, IObject, Boolean>(cls, msg, (dc, d, c) => Dictionary.New(d, c));
			classMessages["new(default:_)"] =
				(cls, msg) => classFunc<DictionaryClass, IObject>(cls, msg, (dc, d) => Dictionary.New(d, false));
			classMessages["empty".get()] = (cls, msg) => classFunc<DictionaryClass>(cls, dc => Dictionary.Empty);
		}

		public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");

		public IObject Revert(IEnumerable<IObject> list) => new Dictionary(list);
	}
}