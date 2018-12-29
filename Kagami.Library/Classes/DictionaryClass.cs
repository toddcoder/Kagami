using Kagami.Library.Objects;
using Standard.Types.Monads;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
	public class DictionaryClass : BaseClass
	{
		public override string Name => "Dictionary";

		public override void RegisterMessages()
		{
			base.RegisterMessages();

			collectionMessages();
			mutableCollectionMessages();

			messages["[]=(_,_)"] = (obj, msg) => function<Dictionary>(obj, d => d[msg.Arguments[0]] = msg.Arguments[1]);
			messages["[](_)"] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, k) => d.Get(k));
			messages["default".get()] = (obj, msg) => function<Dictionary>(obj, d =>
			{
				if (d.DefaultValue.If(out var dv))
					return dv;
				else if (d.DefaultLambda.If(out var dl))
					return dl;
				else
					return Unassigned.Value;
			});
			messages["default".set()] = (obj, msg) => function<Dictionary, IObject>(obj, msg, (d, v) =>
			{
				if (v is Lambda lambda)
					d.DefaultLambda = lambda.Some();
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
	}
}