using System;
using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Classes;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Dates.Now;
using Core.Enumerables;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects
{
	public class Iterator : IObject, IIterator
	{
		protected ICollection collection;
		protected int index;
		protected ICollectionClass collectionClass;

		public Iterator(ICollection collection)
		{
			this.collection = collection;
			if (Module.Global.Class(((IObject)this.collection).ClassName).If(out var baseClass))
				collectionClass = baseClass is ICollectionClass cc ? cc : new ArrayClass();
			else
				baseClass = new ArrayClass();
			index = 0;
		}

		public virtual string ClassName => "Iterator";

		public string AsString => "iterator";

		public virtual string Image => "!iterator";

		public int Hash => ((IObject)collection).Hash;

		public bool IsEqualTo(IObject obj) => isEqualTo(collection, obj);

		public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

		public bool IsTrue => collection.Length.Value > 0;

		public ICollection Collection => collection;

		public ICollectionClass CollectionClass => collectionClass;

		public virtual bool IsLazy => false;

		public virtual IMaybe<IObject> Next() => collection.Next(index++);

		public virtual IMaybe<IObject> Peek() => collection.Peek(index);

		public IObject Reset()
		{
			index = 0;
			return this;
		}

		public virtual IEnumerable<IObject> List()
		{
			var item = none<IObject>();
			index = 0;
			do
			{
				item = Next();
				if (item.If(out var obj))
					yield return obj;

				if (index % 1000 == 0 && Machine.Current.Context.Cancelled())
					yield break;
			} while (item.IsSome);
		}

		public virtual IIterator Clone() => new Iterator(collection);

		public IObject Reverse()
		{
			return collectionClass.Revert(List().Reverse());
		}

		public String Join(string connector) => List().Select(i => i.AsString).Stringify(connector);

		public IObject Sort(Lambda lambda, bool ascending)
		{
			switch (lambda.Invokable.Parameters.Length)
			{
				case 1:
					List<IObject> result;
					if (ascending)
						result = List().OrderBy(i => lambda.Invoke(i)).ToList();
					else
						result = List().OrderByDescending(i => lambda.Invoke(i)).ToList();

					return collectionClass.Revert(result);
				case 2:
					var array = List().ToArray();
					System.Array.Sort(array, (i, j) => ((Int)lambda.Invoke(i, j)).Value);

					return collectionClass.Revert(array);
				default:
					return (IObject)collection;
			}
		}

		public IObject Sort(bool ascending)
		{
			var array = List().ToArray();
			var comparer = new Comparer(ascending);
			System.Array.Sort(array, comparer);

			return collectionClass.Revert(array);
		}

		public IObject FoldLeft(IObject initialValue, Lambda lambda)
		{
			return List().Aggregate(initialValue, (current, value) => lambda.Invoke(current, value));
		}

		public IObject FoldLeft(Lambda lambda)
		{
			var firstObtained = false;
			var current = Unassigned.Value;
			foreach (var value in List())
				if (firstObtained)
					current = lambda.Invoke(current, value);
				else
				{
					current = value;
					firstObtained = true;
				}

			return current;
		}

		public IObject FoldRight(IObject initialValue, Lambda lambda)
		{
			return List().Aggregate(initialValue, (current, value) => lambda.Invoke(value, current));
		}

		public IObject FoldRight(Lambda lambda)
		{
			var firstObtained = false;
			var current = Unassigned.Value;
			foreach (var value in List().Reverse())
				if (firstObtained)
					current = lambda.Invoke(value, current);
				else
				{
					current = value;
					firstObtained = true;
				}

			return current;
		}

		public IObject ReduceLeft(IObject initialValue, Lambda lambda)
		{
			var current = initialValue;
			var result = new List<IObject> { current };
			foreach (var value in List())
			{
				current = lambda.Invoke(current, value);
				result.Add(current);
			}

			return collectionClass.Revert(result);
		}

		public IObject ReduceLeft(Lambda lambda)
		{
			var firstObtained = false;
			var current = Unassigned.Value;
			var result = new List<IObject>();
			foreach (var value in List())
				if (firstObtained)
				{
					current = lambda.Invoke(current, value);
					result.Add(current);
				}
				else
				{
					current = value;
					result.Add(current);
					firstObtained = true;
				}

			return collectionClass.Revert(result);
		}

		public IObject ReduceRight(IObject initialValue, Lambda lambda)
		{
			var current = initialValue;
			var result = new List<IObject> { current };
			foreach (var value in List().Reverse())
			{
				current = lambda.Invoke(value, current);
				result.Add(current);
			}

			return collectionClass.Revert(result);
		}

		public IObject ReduceRight(Lambda lambda)
		{
			var firstObtained = false;
			var current = Unassigned.Value;
			var result = new List<IObject>();
			foreach (var value in List())
				if (firstObtained)
				{
					current = lambda.Invoke(value, current);
					result.Add(current);
				}
				else
				{
					current = value;
					result.Add(current);
					firstObtained = true;
				}

			return collectionClass.Revert(result);
		}

		public Int Count(IObject item) => List().Count(i => i.IsEqualTo(item));

		public Int Count(Lambda predicate)
		{
			var count = 0;
			foreach (var value in List())
				if (predicate.Invoke(value).IsTrue)
					count++;

			return count;
		}

		public virtual IObject Map(Lambda lambda) => collectionClass.Revert(List().Select(value => lambda.Invoke(value)));

		public virtual IObject FlatMap(Lambda lambda)
		{
			var className = ((BaseClass)collectionClass).Name;
			var enumerable = List().Select(value => lambda.Invoke(value));
			var flattened = flatten(enumerable, className);

			return collectionClass.Revert(flattened);
		}

		public IObject MapIf(Lambda lambda, Lambda predicate)
		{
			var list = new List<IObject>();
			foreach (var item in List())
				if (predicate.Invoke(item).IsTrue)
					list.Add(lambda.Invoke(item));
				else
					list.Add(item);

			return collectionClass.Revert(list);
		}

		public virtual IObject If(Lambda predicate) => collectionClass.Revert(List().Where(value => predicate.Invoke(value).IsTrue));

		public virtual IObject IfNot(Lambda predicate) => collectionClass.Revert(List().Where(value => !predicate.Invoke(value).IsTrue));

		public virtual IObject Skip(int count)
		{
			if (count > -1)
				return collectionClass.Revert(List().Skip(count));
			else
			{
				var list = List().ToList();
				return collectionClass.Revert(list.Take(list.Count + count));
			}
		}

		public virtual IObject SkipWhile(Lambda predicate)
		{
			return collectionClass.Revert(List().SkipWhile(value => predicate.Invoke(value).IsTrue));
		}

		public virtual IObject SkipUntil(Lambda predicate)
		{
			return collectionClass.Revert(List().SkipWhile(value => !predicate.Invoke(value).IsTrue));
		}

		public virtual IObject Take(int count)
		{
			if (count > -1)
				return collectionClass.Revert(List().Take(count));
			else
			{
				var list = List().ToList();
				return collectionClass.Revert(list.Skip(list.Count + count));
			}
		}

		public virtual IObject TakeWhile(Lambda predicate)
		{
			return collectionClass.Revert(List().TakeWhile(value => predicate.Invoke(value).IsTrue));
		}

		public virtual IObject TakeUntil(Lambda predicate)
		{
			return collectionClass.Revert(List().TakeWhile(value => !predicate.Invoke(value).IsTrue));
		}

		public IObject Index(Lambda predicate)
		{
			var i = 0;
			foreach (var value in List())
			{
				if (predicate.Invoke(value).IsTrue)
					return new Some((Int)i);

				i++;
			}

			return Objects.None.NoneValue;
		}

		public IObject Indexes(Lambda predicate)
		{
			var i = 0;
			var result = new List<IObject>();
			foreach (var value in List())
			{
				if (predicate.Invoke(value).IsTrue)
					result.Add((Int)i);

				i++;
			}

			return collectionClass.Revert(result);
		}

		public IObject Zip(ICollection collection)
		{
			var rightList = collection.GetIterator(false).List();
			return collectionClass.Revert(List().Zip(rightList, (x, y) => collectionClass.Revert(new List<IObject> { x, y })));
		}

		public IObject Zip(ICollection collection, Lambda lambda)
		{
			var rightList = collection.GetIterator(false).List();
			return collectionClass.Revert(List().Zip(rightList, (x, y) => lambda.Invoke(x, y)));
		}

		public IObject Min()
		{
			var result = Unassigned.Value;
			foreach (var value in List())
				if (result is Unassigned)
					switch (value)
					{
						case IObjectCompare _:
							result = value;
							break;
						default:
							return Unassigned.Value;
					}
				else if (value is IObjectCompare oc && oc.Compare(result) < 0)
					result = value;

			return result;
		}

		public IObject Min(Lambda lambda)
		{
			var result = Unassigned.Value;
			if (lambda.Invokable.Parameters.Length == 2)
			{
				foreach (var value in List())
					if (result is Unassigned)
						result = value;
					else if (((Int)lambda.Invoke(value, result)).Value < 0)
						result = value;
			}
			else
			{
				var list = List().ToList();
				result = list[0];
				var compareResult = lambda.Invoke(result);
				foreach (var value in list.Skip(1))
				{
					var valueResult = lambda.Invoke(value);
					if (valueResult is IObjectCompare oc)
					{
						if (oc.Compare(compareResult) < 0)
						{
							result = value;
							compareResult = valueResult;
						}
					}
					else
						throw incompatibleClasses(valueResult, "Object compare");
				}
			}

			return result;
		}

		public IObject Max()
		{
			var result = Unassigned.Value;
			foreach (var value in List())
				if (result is Unassigned)
					switch (value)
					{
						case IObjectCompare _:
							result = value;
							break;
						default:
							return Unassigned.Value;
					}
				else if (value is IObjectCompare oc && oc.Compare(result) > 0)
					result = value;

			return result;
		}

		public IObject Max(Lambda lambda)
		{
			var result = Unassigned.Value;
			if (lambda.Invokable.Parameters.Length == 2)
			{
				foreach (var value in List())
					if (result is Unassigned)
						result = value;
					else if (((Int)lambda.Invoke(value, result)).Value < 0)
						result = value;
			}
			else
			{
				var list = List().ToList();
				result = list[0];
				var compareResult = lambda.Invoke(result);
				foreach (var value in list.Skip(1))
				{
					var valueResult = lambda.Invoke(value);
					if (valueResult is IObjectCompare oc)
					{
						if (oc.Compare(compareResult) > 0)
						{
							result = value;
							compareResult = valueResult;
						}
					}
					else
						throw incompatibleClasses(valueResult, "Object compare");
				}
			}

			return result;
		}

		public IObject First() => List().FirstOrNone().FlatMap(value => new Some(value), () => Objects.None.NoneValue);

		public IObject First(Lambda predicate)
		{
			foreach (var value in List())
				if (predicate.Invoke(value).IsTrue)
					return new Some(value);

			return Objects.None.NoneValue;
		}

		public IObject Last() => List().Reverse().FirstOrNone().FlatMap(value => new Some(value), () => Objects.None.NoneValue);

		public IObject Last(Lambda predicate)
		{
			foreach (var value in List().Reverse())
				if (predicate.Invoke(value).IsTrue)
					return new Some(value);

			return Objects.None.NoneValue;
		}

		public IObject Split(Lambda predicate)
		{
			var ifTrue = new List<IObject>();
			var ifFalse = new List<IObject>();
			foreach (var value in List())
				if (predicate.Invoke(value).IsTrue)
					ifTrue.Add(value);
				else
					ifFalse.Add(value);

			return collectionClass.Revert(new List<IObject> { collectionClass.Revert(ifTrue), collectionClass.Revert(ifFalse) });
		}

		public IObject Split(int count)
		{
			var ifTrue = new List<IObject>();
			var ifFalse = new List<IObject>();
			var i = 0;
			foreach (var value in List())
				if (i++ < count)
					ifTrue.Add(value);
				else
					ifFalse.Add(value);

			return collectionClass.Revert(new List<IObject> { collectionClass.Revert(ifTrue), collectionClass.Revert(ifFalse) });
		}

		public virtual IObject GroupBy(Lambda lambda)
		{
			var hash = new AutoHash<IObject, List<IObject>>(o => new List<IObject>()) { AutoAddDefault = true };
			foreach (var item in List())
			{
				var key = lambda.Invoke(item);
				hash[key].Add(item);
			}

			var result = new Hash<IObject, IObject>();

			foreach (var key in hash.KeyArray())
				result[key] = collectionClass.Revert(hash[key]);

			return new Dictionary(result);
		}

		public Boolean One(Lambda predicate)
		{
			var one = false;
			foreach (var value in List())
				if (predicate.Invoke(value).IsTrue)
				{
					if (one)
						return new Boolean(false);
					else
						one = true;
				}

			return new Boolean(true);
		}

		public Boolean None(Lambda predicate)
		{
			foreach (var value in List())
				if (predicate.Invoke(value).IsTrue)
					return false;

			return true;
		}

		public Boolean Any(Lambda predicate) => List().Any(value => predicate.Invoke(value).IsTrue);

		public Boolean All(Lambda predicate) => List().All(value => predicate.Invoke(value).IsTrue);

		public INumeric Sum()
		{
			var sum = (INumeric)Int.Zero;
			foreach (var value in List())
				if (value is INumeric numeric)
					sum = (INumeric)apply(sum, numeric, (x, y) => x + y, (x, y) => x + y, (x, y) => x + y, (x, y) => x.Add(y), "+");

			return sum;
		}

		public INumeric Average()
		{
			var sum = Sum();
			var length = collection.GetIterator(false).List().Count(value => value is INumeric);

			return (INumeric)apply(sum, (Int)length, (x, y) => x / y, (x, y) => x / y, (x, y) => x / y, (x, y) => x.Divide(y), "/");
		}

		public INumeric Product()
		{
			var product = (INumeric)Int.One;
			foreach (var value in List())
				if (value is INumeric numeric)
					product = (INumeric)apply(product, numeric, (x, y) => x * y, (x, y) => x * y, (x, y) => x * y, (x, y) => x.Multiply(y),
						"*");

			return product;
		}

		public IObject Cross(ICollection collection)
		{
			var result = new List<List<IObject>>();
			foreach (var left in List())
			foreach (var right in collection.GetIterator(false).List())
			{
				var inner = new List<IObject> { left, right };
				result.Add(inner);
			}

			return collectionClass.Revert(result.Select(l => collectionClass.Revert(l)));
		}

		public IObject By(int count)
		{
			if (count > 1)
			{
				var outer = new List<IObject>();
				var inner = new List<IObject>();
				foreach (var value in List())
				{
					inner.Add(value);
					if (inner.Count == count)
					{
						outer.Add(collectionClass.Revert(inner));
						inner.Clear();
					}
				}

				if (inner.Count > 0)
					outer.Add(collectionClass.Revert(inner));
				return collectionClass.Revert(outer);
			}
			else
				return collectionClass.Revert(List());
		}

		public IObject Window(int count)
		{
			if (count > 1)
			{
				var list = List().ToList();
				if (list.Count >= count)
				{
					var lastIndex = list.Count - 1;
					var outerList = new List<IObject>();
					var escape = false;
					for (var i = 0; i < list.Count && !escape; i++)
					{
						var innerList = new List<IObject>();
						for (var j = i; j < i + count; j++)
						{
							innerList.Add(list[j]);
							if (j == lastIndex)
								escape = true;
						}

						var result = collectionClass.Revert(innerList);
						outerList.Add(result);
					}

					return collectionClass.Revert(outerList);
				}
				else
					return collectionClass.Revert(list);
			}
			else
				return collectionClass.Revert(List());
		}

		public virtual IObject Distinct() => collectionClass.Revert(List().Distinct());

		public IObject Span(Lambda predicate)
		{
			var whileTrue = true;
			var isTrue = new List<IObject>();
			var isFalse = new List<IObject>();

			foreach (var value in List())
				if (whileTrue && predicate.Invoke(value).IsTrue)
					isTrue.Add(value);
				else if (whileTrue)
				{
					whileTrue = false;
					isFalse.Add(value);
				}
				else
					isFalse.Add(value);

			return collectionClass.Revert(new List<IObject> { collectionClass.Revert(isTrue), collectionClass.Revert(isFalse) });
		}

		public IObject Span(int count)
		{
			var isTrue = new List<IObject>();
			var isFalse = new List<IObject>();

			foreach (var value in List())
				if (isTrue.Count < count)
					isTrue.Add(value);
				else
					isFalse.Add(value);

			return collectionClass.Revert(new List<IObject> { collectionClass.Revert(isTrue), collectionClass.Revert(isFalse) });
		}

		public IObject Shuffle()
		{
			var array = List().ToArray();
			return shuffle(array, array.Length);
		}

		public IObject Random()
		{
			var array = List().ToArray();
			var random = new Random(NowServer.Now.Millisecond);
			var i = random.Next(array.Length);

			return array[i];
		}

		public IObject Collect() => collectionClass.Revert(List());

		public Array ToArray() => new Array(List());

		public List ToList() => Objects.List.NewList(List());

		public Tuple ToTuple() => new Tuple(List().ToArray());

		public Dictionary ToDictionary(Lambda keyLambda, Lambda valueLambda)
		{
			var hash = new Hash<IObject, IObject>();

			foreach (var item in List())
			{
				var key = keyLambda.Invoke(item);
				var value = valueLambda.Invoke(item);
				hash[key] = value;
			}

			return new Dictionary(hash);
		}

		public IObject ToDictionary() => Array.CreateObject(List());

		public IObject Each(Lambda action)
		{
			foreach (var item in List())
				action.Invoke(item);
			return this;
		}

		public IObject Rotate(int count)
		{
			var postfix = new List<IObject>();
			var item = Next();
			for (var i = 0; i < count; i++)
			{
				if (item.If(out var obj))
					postfix.Add(obj);
				item = Next();
			}

			var result = new List<IObject>();

			while (item.If(out var obj))
			{
				result.Add(obj);
				item = Next();
			}

			result.AddRange(postfix);

			return collectionClass.Revert(result);
		}

		static void rotateRight(List<IObject> list, int count)
		{
			var temp = list[count - 1];
			list.RemoveAt(count - 1);
			list.Insert(0, temp);
		}

		IEnumerable<List<IObject>> permutate(List<IObject> list, int count)
		{
			if (count == 1)
				yield return list;
			else
				for (var i = 0; i < count; i++)
					foreach (var perm in permutate(list, count - 1))
					{
						yield return perm;

						rotateRight(list, count);
					}
		}

		public IObject Permutation(int count)
		{
			var list = List().ToList();
			var enumerable = permutate(list, count).Select(l => collectionClass.Revert(l));

			return collectionClass.Revert(enumerable);
		}

		static void rotateLeft(List<IObject> list, int start, int count)
		{
			var temp = list[start];
			list.RemoveAt(start);
			list.Insert(start + count - 1, temp);
		}

		static IEnumerable<List<IObject>> combinations(List<IObject> list, int start, int count, int choose)
		{
			if (choose == 0)
				yield return list;
			else
				for (var i = 0; i < count; i++)
				{
					foreach (var combo in combinations(list, start + 1, count - 1 - i, choose - 1))
						yield return combo;

					rotateLeft(list, start, count);
				}
		}

		public IObject Combination(int count)
		{
			var list = List().ToList();
			var result = combinations(list, 0, list.Count, count);

			return collectionClass.Revert(result.Select(l => collectionClass.Revert(l)));
		}

		static IEnumerable<IObject> flatten(IIterator iterator)
		{
			var className = ((IObject)iterator.Collection).ClassName;

			while (iterator.Next().If(out var item))
				if (item.ClassName == className)
				{
					var innerIterator = ((ICollection)item).GetIterator(false);
					foreach (var inner in flatten(innerIterator))
						yield return inner;
				}
				else
					yield return item;
		}

		static IEnumerable<IObject> flatten(IEnumerable<IObject> enumerable, string className)
		{
			foreach (var item in enumerable)
				if (item.ClassName == className)
				{
					var innerIterator = ((ICollection)item).GetIterator(false);
					foreach (var inner in flatten(innerIterator))
						yield return inner;
				}
				else
					yield return item;
		}

		public IObject Flatten() => collectionClass.Revert(flatten(this));

		public IObject Copy() => collectionClass.Revert(List());

		public IObject Apply(ICollection collection)
		{
			var lambdas = List().Select(l => (Lambda)l).ToList();
			var list = collection.GetIterator(false).List().ToList();

			var result = applyAgainst(lambdas, list).ToList();
			return collectionClass.Revert(result);
		}

		static IEnumerable<IObject> applyAgainst(List<Lambda> lambdas, List<IObject> enumerable)
		{
			foreach (var lambda in lambdas)
			foreach (var item in enumerable)
				yield return lambda.Invoke(item);
		}

		IObject shuffle(IObject[] array, int count)
		{
			var result = new Hash<int, IObject>();
			var random = new Random(NowServer.Now.Millisecond);
			for (var i = 0; i < count; i++)
			{
				var key = random.Next(array.Length);
				while (result.ContainsKey(key))
					key = random.Next(array.Length);
				result[key] = array[key];
			}

			return collectionClass.Revert(result.ValueArray());
		}

		public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");
	}
}