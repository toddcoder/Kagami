using System;
using System.Collections.Generic;
using System.Linq;
using Core.Enumerables;
using Core.Numbers;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
	public static class CollectionFunctions
	{
		public static bool isEqualTo<T>(T left, IObject right) where T : ICollection
		{
			if (right is T other)
			{
				var l1 = left.GetIterator(false).List().ToArray();
				var l2 = other.GetIterator(false).List().ToArray();
				if (l1.Length == l2.Length)
				{
					return l1.Zip(l2, (a, b) => a.IsEqualTo(b)).All(b => b);
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		public static int compareCollections<T>(T left, IObject right) where T : ICollection, IObjectCompare
		{
			if (right is T other)
			{
				var l1 = left.GetIterator(false).List().ToArray();
				var l2 = other.GetIterator(false).List().ToArray();
				var length1 = l1.Length;
				var length2 = l2.Length;
				var length = Math.Min(length1, length2);
				for (var i = 0; i < length; i++)
				{
					var item1 = l1[i];
					var item2 = l2[i];
					if (item1 is IObjectCompare oc)
					{
						var compare = oc.Compare(item2);
						if (compare != 0)
						{
							return compare;
						}
					}
					else
					{
						throw incompatibleClasses(item1, "Object compare");
					}
				}

				if (length1 == length2)
				{
					return 0;
				}
				else if (length1 < length2)
				{
					return -1;
				}
				else
				{
					return 1;
				}
			}
			else
			{
				throw incompatibleClasses(right, typeof(T).Name);
			}
		}

		public static String makeString(ICollection collection, string connector)
		{
			return collection.GetIterator(false).List().Select(i => i.AsString).Stringify(connector);
		}

		public static IEnumerable<int> indexList(Container container, int length)
		{
			return container.List
				.Cast<Int>()
				.Select(i => wrapIndex(i.Value, length))
				.Where(i => i.Between(0).Until(length));
		}

		static Container conditionContainer(Container container)
		{
			var list = new List<IObject>();
			foreach (var obj in container.List)
			{
				switch (obj)
				{
					case ICollection collection:
						foreach (var obj2 in collection.GetIterator(false).List())
						{
							list.Add(obj2);
						}

						break;
					case IIterator iterator:
						foreach (var innerObject in iterator.List())
						{
							list.Add(innerObject);
						}

						break;
					default:
						list.Add(obj);
						break;
				}
			}

			return new Container(list);
		}

		public static IObject getIndexed<T>(T obj, IObject index, Func<T, int, IObject> intGetter,
			Func<T, Container, IObject> listGetter) where T : IObject
		{
			switch (index)
			{
				case Int i:
					return intGetter(obj, i.Value);
				case Container internalList:
					return listGetter(obj, conditionContainer(internalList));
				case ICollection collection when !(index is String):
					return listGetter(obj, new Container(collection.GetIterator(false).List()));
				case IIterator iterator:
					return listGetter(obj, new Container(iterator.List()));
				default:
					throw invalidIndex(index);
			}
		}

		public static IObject setIndexed<T>(T obj, IObject index, IObject value, Action<T, int, IObject> intSetter,
			Action<T, Container, IObject> listSetter) where T : IObject
		{
			switch (index)
			{
				case Int i:
					intSetter(obj, i.Value, value);
					return obj;
				case Container internalList:
					listSetter(obj, conditionContainer(internalList), value);
					return obj;
				case ICollection collection when !(index is String):
					listSetter(obj, new Container(collection.GetIterator(false).List()), value);
					return obj;
				case IIterator iterator:
					listSetter(obj, new Container(iterator.List()), value);
					return obj;
				default:
					throw invalidIndex(index);
			}
		}
	}
}