using System.Collections.Generic;
using Kagami.Library.Objects;
using Core.Exceptions;
using Core.Monads;
using static Kagami.Library.Classes.ClassFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using Array = Kagami.Library.Objects.Array;
using Boolean = Kagami.Library.Objects.Boolean;
using Void = Kagami.Library.Objects.Void;

namespace Kagami.Library.Classes
{
	public class ArrayClass : BaseClass, ICollectionClass
	{
		public override string Name => "Array";

		public IObject Revert(IEnumerable<IObject> list) => Array.CreateObject(list);

		public override void RegisterMessages()
		{
			base.RegisterMessages();

			collectionMessages();
			messages["array()"] = (obj, msg) => function<Array>(obj, a => a);
			mutableCollectionMessages();
			sliceableMessages();
			skipTakeableMessages();

			messages["[](_)"] = (obj, msg) => function<Array, IObject>(obj, msg, getIndexed);
			messages["get(_)"] = (obj, msg) => function<Array, IObject>(obj, msg, (a, i) => someOf(a.Get(i)));
			messages["[]=(_,_)"] = (obj, msg) => function<Array, IObject, IObject>(obj, msg, setIndexed);
			messages["~(_)"] = (obj, msg) => function<Array, Array>(obj, msg, (a1, a2) => a1.Concatenate(a2));
			registerMessage("push(_)", (obj, msg) => function<Array, IObject>(obj, msg, (a, v) => a.Append(v)));
			registerMessage("pop()", (obj, msg) => function<Array>(obj, a => a.Pop()));
			registerMessage("unshift(_)", (obj, msg) => function<Array, IObject>(obj, msg, (a, v) => a.Unshift(v)));
			registerMessage("shift()", (obj, msg) => function<Array>(obj, a => a.Shift()));
			messages["find".Selector("<Array>", "startAt:<Int>", "reverse:<Boolean>")] = (obj, msg) =>
				function<Array, IObject, Int, Boolean>(obj, msg, (a, o, i, r) => a.Find(o, i.Value, r.Value));
			messages["find".Selector("<Array>", "startAt:<Int>")] = (obj, msg) =>
				function<Array, IObject, Int>(obj, msg, (a, o, i) => a.Find(o, i.Value, false));
			messages["find".Selector("<Array>", "reverse:<Boolean>")] = (obj, msg) =>
				function<Array, IObject, Boolean>(obj, msg, (a, o, r) => a.Find(o, 0, r.Value));
			messages["find"] = (obj, msg) =>
				function<Array, IObject>(obj, msg, (a, o) => a.Find(o, 0, false));
			messages["find".Selector("all:<Array>")] = (obj, msg) =>
				function<Array, IObject>(obj, msg, (a, o) => a.FindAll(o));
			messages["default".get()] = (obj, msg) => function<Array>(obj, array =>
			{
				if (array.DefaultValue.If(out var dv))
				{
					return dv;
				}
				else if (array.DefaultLambda.If(out var dl))
				{
					return dl;
				}
				else
				{
					return Unassigned.Value;
				}
			});
			messages["default".set()] = (obj, msg) => function<Array, IObject>(obj, msg, (array, v) =>
			{
				if (v is Lambda lambda)
				{
					array.DefaultLambda = lambda.Some();
				}

				array.DefaultValue = v.Some();

				return Void.Value;
			});
			messages["transpose()"] = (obj, msg) => function<Array>(obj, a => a.Transpose());
		}

		static IObject getIndexed(Array a, IObject i)
		{
			return CollectionFunctions.getIndexed(a, i, (array, index) => array[index], (array, list) => array[list]);
		}

		static IObject setIndexed(Array a, IObject i, IObject v)
		{
			CollectionFunctions.setIndexed(a, i, v, (array, index, value) => array[index] = value,
				(array, list, value) => array[list] = value);
			return a;
		}

		public override void RegisterClassMessages()
		{
			base.RegisterClassMessages();

			classMessages["repeat(value:_,times:_<Int>)"] = (bc, msg) =>
				classFunc<ArrayClass, IObject, Int>(bc, msg, (ac, v, t) => Array.Repeat(v, t.Value));
			classMessages["empty".get()] = (bc, msg) => classFunc<ArrayClass>(bc, ac => Array.Empty);
			classMessages["typed"] = (bc, msg) => getTypedArray(msg);
		}

		static Array getTypedArray(Message message)
		{
			if (message.Arguments[0] is TypeConstraint typeConstraint)
			{
				return new Array(new IObject[0]) { TypeConstraint = typeConstraint.Some() };
			}
			else
			{
				throw "Type constraint for array required".Throws();
			}
		}

		public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");
	}
}