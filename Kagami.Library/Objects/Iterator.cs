using Core.Collections;
using Core.Dates.Now;
using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Classes;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Objects;

public class Iterator : IObject, IIterator
{
   protected ICollection collection;
   protected Maybe<TypeConstraint> _typeConstraint = nil;
   protected int index;
   protected ICollectionClass collectionClass;

   public Iterator(ICollection collection)
   {
      this.collection = collection;
      _typeConstraint = this.collection.TypeConstraint;
      collectionClass = Module.CollectionClass(collection);
      index = 0;
   }

   public virtual string ClassName => "Iterator";

   public virtual string AsString => "it collection";

   public virtual string Image => "it collection";

   public int Hash => ((IObject)collection).Hash;

   public bool IsEqualTo(IObject obj) => isEqualTo(collection, obj);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => collection.Length.Value > 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public ICollection Collection => collection;

   public ICollectionClass CollectionClass => collectionClass;

   public virtual bool IsLazy => false;

   public virtual Maybe<IObject> Next() => collection.Next(index++);

   public virtual Maybe<IObject> Peek() => collection.Peek(index);

   public virtual IObject Reset()
   {
      index = 0;
      return this;
   }

   public virtual IEnumerable<IObject> List()
   {
      index = 0;
      do
      {
         var _item = Next();
         if (_item is (true, var item))
         {
            yield return item;
         }
         else
         {
            break;
         }

         if (index % 1000 == 0 && Machine.Current.Context.Cancelled())
         {
            yield break;
         }
      } while (true);
   }

   public virtual IIterator Clone() => new Iterator(collection);

   public IObject Reverse()
   {
      var list = List().ToList();
      list.Reverse();

      return collectionClass.Revert(list, _typeConstraint);
   }

   public KString Join() => List().Select(i => i.AsString).ToString("");

   public KString Join(string connector) => List().Select(i => i.AsString).ToString(connector);

   public KString Join(string connector, int limit, string truncated)
   {
      var list = List().ToList();
      if (list.Count > limit)
      {
         var truncatedList = list.Take(limit).ToList();
         truncatedList.Add(new KString(truncated));
         return truncatedList.Select(i => i.AsString).ToString(connector);
      }
      else
      {
         return list.Select(i => i.AsString).ToString(connector);
      }
   }

   public IObject Join(Lambda lambda) => FoldLeft(lambda);

   public KString Join(string connector, string prefix, string suffix)
   {
      return List().Select(i => i.AsString).Select(i => $"{prefix}{i}{suffix}").ToString(connector);
   }

   public IObject Sort(Lambda lambda, bool ascending)
   {
      switch (lambda.Invokable.Parameters.Length)
      {
         case 1 when ascending:
         {
            var result = List().OrderBy(i => lambda.Invoke(i), new ObjectComparer());
            return collectionClass.Revert(result, _typeConstraint);
         }
         case 1:
         {
            var result = List().OrderByDescending(i => lambda.Invoke(i), new ObjectComparer());
            return collectionClass.Revert(result, _typeConstraint);
         }
         case 2 when ascending:
         {
            IObject[] array = [.. List()];
            Array.Sort(array, (i, j) => ((Int)lambda.Invoke(i, j)).Value);

            return collectionClass.Revert(array, _typeConstraint);
         }
         case 2:
         {
            IObject[] array = [.. List()];
            Array.Sort(array, (i, j) => -((Int)lambda.Invoke(i, j)).Value);
            return collectionClass.Revert(array, _typeConstraint);
         }
         default:
            return (IObject)collection;
      }
   }

   public IObject Sort(bool ascending)
   {
      var array = List().ToArray();
      var comparer = new Comparer(ascending);
      Array.Sort(array, comparer);

      return collectionClass.Revert(array, _typeConstraint);
   }

   public IObject FoldLeft(IObject initialValue, Lambda lambda)
   {
      var accum = initialValue;
      foreach (var item in List())
      {
         accum = lambda.Invoke(accum, item);
      }

      return accum;
   }

   public IObject FoldLeft(Lambda lambda)
   {
      var firstObtained = false;
      var current = Unassigned.Value;
      foreach (var value in List())
      {
         if (firstObtained)
         {
            current = lambda.Invoke(current, value);
         }
         else
         {
            current = value;
            firstObtained = true;
         }
      }

      return current;
   }

   public IObject FoldRight(IObject initialValue, Lambda lambda)
   {
      var accum = initialValue;
      foreach (var item in List())
      {
         accum = lambda.Invoke(item, accum);
      }

      return accum;
   }

   public IObject FoldRight(Lambda lambda)
   {
      var firstObtained = false;
      var current = Unassigned.Value;
      var list = List().ToList();
      list.Reverse();
      foreach (var value in list)
      {
         if (firstObtained)
         {
            current = lambda.Invoke(value, current);
         }
         else
         {
            current = value;
            firstObtained = true;
         }
      }

      return current;
   }

   public IObject ReduceLeft(IObject initialValue, Lambda lambda)
   {
      var current = initialValue;
      List<IObject> result = [current];
      foreach (var value in List())
      {
         current = lambda.Invoke(current, value);
         result.Add(current);
      }

      return collectionClass.Revert(result, _typeConstraint);
   }

   public IObject ReduceLeft(Lambda lambda)
   {
      var firstObtained = false;
      var current = Unassigned.Value;
      List<IObject> result = [];
      foreach (var value in List())
      {
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
      }

      return collectionClass.Revert(result, _typeConstraint);
   }

   public IObject ReduceRight(IObject initialValue, Lambda lambda)
   {
      var current = initialValue;
      List<IObject> result = [current];
      var list = List().ToList();
      list.Reverse();
      foreach (var value in list)
      {
         current = lambda.Invoke(value, current);
         result.Add(current);
      }

      return collectionClass.Revert(result, _typeConstraint);
   }

   public IObject ReduceRight(Lambda lambda)
   {
      var firstObtained = false;
      var current = Unassigned.Value;
      List<IObject> result = [];
      foreach (var value in List())
      {
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
      }

      return collectionClass.Revert(result, _typeConstraint);
   }

   public Int Count() => List().Count();

   public Int Count(IObject item) => List().Count(i => i.IsEqualTo(item));

   public Int Count(Lambda predicate) => List().Count(value => predicate.Invoke(value).IsTrue);

   public virtual IObject Map(Lambda lambda) => collectionClass.Revert(List().Select(value => lambda.Invoke(value)), nil);

   public virtual IObject FlatMap(Lambda lambda)
   {
      var newCollection = collectionClass.Revert(List(), nil);
      return new FlatMapIterator((ICollection)newCollection).FlatMap(lambda);
   }

   public IObject MapAll(Lambda lambda)
   {
      List<IObject> list = [];
      foreach (var item in List())
      {
         if (item is ICollection innerCollection)
         {
            var innerCollectionClass = (ICollectionClass)classOf((IObject)innerCollection);
            var iterator = innerCollection.GetIterator(false);
            var mappedItem = iterator.List().Select(i => lambda.Invoke(i));
            var newCollection = innerCollectionClass.Revert(mappedItem, nil);
            list.Add(newCollection);
         }
         else
         {
            list.Add(item);
         }
      }

      return collectionClass.Revert(list, nil);
   }

   public IObject MapIf(Lambda lambda)
   {
      List<IObject> result = [];
      foreach (var item in List())
      {
         var monad = lambda.Invoke(item);
         switch (monad)
         {
            case Some some:
               result.Add(some.Value);
               break;
            case Success success:
               result.Add(success.Value);
               break;
            default:
               result.Add(item);
               break;
         }
      }

      return collectionClass.Revert(result, _typeConstraint);
   }

   public IObject Replace(Lambda predicate, Lambda lambda)
   {
      var list = new List<IObject>();
      foreach (var item in List())
      {
         list.Add(predicate.Invoke(item).IsTrue ? lambda.Invoke(item) : item);
      }

      return collectionClass.Revert(list, _typeConstraint);
   }

   public virtual IObject If(Lambda predicate) =>
      collectionClass.Revert(List().Where(value => predicate.Invoke(value).IsTrue), _typeConstraint);

   public virtual IObject IfNot(Lambda predicate) =>
      collectionClass.Revert(List().Where(value => !predicate.Invoke(value).IsTrue), _typeConstraint);

   public virtual IObject Skip(int count)
   {
      if (count > -1)
      {
         return collectionClass.Revert(List().Skip(count), _typeConstraint);
      }
      else
      {
         var list = List().ToList();
         return collectionClass.Revert(list.Take(list.Count + count), _typeConstraint);
      }
   }

   public virtual IObject SkipWhile(Lambda predicate, bool back)
   {
      var list = List();
      IEnumerable<IObject> result;
      if (back)
      {
         result = list.Reverse().SkipWhile(value => predicate.Invoke(value).IsTrue).Reverse();
      }
      else
      {
         result = list.SkipWhile(value => predicate.Invoke(value).IsTrue);
      }

      return collectionClass.Revert(result, _typeConstraint);
   }

   public virtual IObject SkipUntil(Lambda predicate, bool back)
   {
      var list = List();
      IEnumerable<IObject> result;
      if (back)
      {
         result = list.Reverse().SkipWhile(value => !predicate.Invoke(value).IsTrue).Reverse();
      }
      else
      {
         result = list.SkipWhile(value => !predicate.Invoke(value).IsTrue);
      }

      return collectionClass.Revert(result, _typeConstraint);
   }

   public virtual IObject Take(int count)
   {
      if (count > -1)
      {
         return collectionClass.Revert(List().Take(count), _typeConstraint);
      }
      else
      {
         var list = List().ToList();
         return collectionClass.Revert(list.Skip(list.Count + count), _typeConstraint);
      }
   }

   public virtual IObject TakeWhile(Lambda predicate, bool back)
   {
      var list = List();
      IEnumerable<IObject> result;
      if (back)
      {
         result = list.Reverse().TakeWhile(value => predicate.Invoke(value).IsTrue).Reverse();
      }
      else
      {
         result = list.TakeWhile(value => predicate.Invoke(value).IsTrue);
      }

      return collectionClass.Revert(result, _typeConstraint);
   }

   public virtual IObject TakeUntil(Lambda predicate, bool back)
   {
      var list = List();
      IEnumerable<IObject> result;
      if (back)
      {
         result = list.Reverse().TakeWhile(value => !predicate.Invoke(value).IsTrue).Reverse();
      }
      else
      {
         result = list.TakeWhile(value => !predicate.Invoke(value).IsTrue);
      }

      return collectionClass.Revert(result, _typeConstraint);
   }

   public IObject Index(Lambda predicate)
   {
      var i = 0;
      foreach (var value in List())
      {
         if (predicate.Invoke(value).IsTrue)
         {
            return new Some((Int)i);
         }

         i++;
      }

      return KNil.NilValue;
   }

   public IObject Indexes(Lambda predicate)
   {
      var i = 0;
      List<IObject> result = [];
      foreach (var value in List())
      {
         if (predicate.Invoke(value).IsTrue)
         {
            result.Add((Int)i);
         }

         i++;
      }

      return collectionClass.Revert(result, _typeConstraint);
   }

   public virtual IObject Zip(ICollection collection) => Zip(collection.GetIterator(false));

   public IObject Zip(IIterator iterator)
   {
      List<IObject> result = [];
      var leftIterator = collection.GetIterator(false);
      while (leftIterator.Next() is (true, var next))
      {
         var _rightNext = iterator.Next();
         if (_rightNext is (true, var rightNext))
         {
            if (next is IMutableCollection mutableCollection)
            {
               var rightIterator = alwaysAnIterator(rightNext, false);
               foreach (var rightItem in rightIterator.List())
               {
                  mutableCollection.Append(rightItem);
               }

               result.Add(next);
            }
            else
            {
               var resultItem = collectionClass.Revert([next, rightNext], _typeConstraint);
               result.Add(resultItem);
            }
         }
      }

      return collectionClass.Revert(result, _typeConstraint);
   }

   public IObject Zip(OpenRange openRange)
   {
      List<IObject> result = [];
      var leftIterator = collection.GetIterator(false);
      var rightIterator = openRange.GetIterator(true);
      while (leftIterator.Next() is (true, var left) && rightIterator.Next() is (true, var right))
      {
         result.Add(collectionClass.Revert([left, right], nil));
      }

      return collectionClass.Revert(result, nil);
   }

   public IObject Zip(NumericOpenRange openRange)
   {
      List<IObject> result = [];
      var leftIterator = collection.GetIterator(false);
      var rightIterator = openRange.GetIterator(true);
      while (leftIterator.Next() is (true, var left) && rightIterator.Next() is (true, var right))
      {
         result.Add(collectionClass.Revert([left, right], nil));
      }

      return collectionClass.Revert(result, nil);
   }

   public virtual IObject Zip(ICollection collection, Lambda lambda) => Zip(collection.GetIterator(false), lambda);

   public IObject Zip(IIterator iterator, Lambda lambda)
   {
      List<IObject> result = [];
      var leftIterator = collection.GetIterator(false);
      while (leftIterator.Next() is (true, var next))
      {
         var _rightNext = iterator.Next();
         if (_rightNext is (true, var rightNext))
         {
            var resultItem = lambda.Invoke(next, rightNext);
            result.Add(resultItem);
         }
      }

      return collectionClass.Revert(result, _typeConstraint);
   }

   public IObject Zip(OpenRange openRange, Lambda lambda)
   {
      List<IObject> result = [];
      var leftIterator = collection.GetIterator(false);
      var rightIterator = openRange.GetIterator(true);
      while (leftIterator.Next() is (true, var left) && rightIterator.Next() is (true, var right))
      {
         result.Add(lambda.Invoke(left, right));
      }

      return collectionClass.Revert(result, nil);
   }

   public IObject Zip(NumericOpenRange openRange, Lambda lambda)
   {
      List<IObject> result = [];
      var leftIterator = collection.GetIterator(false);
      var rightIterator = openRange.GetIterator(true);
      while (leftIterator.Next() is (true, var left) && rightIterator.Next() is (true, var right))
      {
         result.Add(lambda.Invoke(left, right));
      }

      return collectionClass.Revert(result, nil);
   }

   public virtual IObject ZipL(ICollection collection, IObject leftDefaultValue, IObject rightDefaultValue)
   {
      return collectionClass.Revert(zipUnequal(List(), collection.GetIterator(false).List())
         .Select(t => collectionClass.Revert(new List<IObject> { t.left, t.right }, _typeConstraint)), _typeConstraint);

      IEnumerable<(IObject left, IObject right)> zipUnequal(IEnumerable<IObject> left, IEnumerable<IObject> right)
      {
         using var leftEnumerator = left.GetEnumerator();
         using var rightEnumerator = right.GetEnumerator();
         while (true)
         {
            var leftMoveNext = leftEnumerator.MoveNext();
            var rightMoveNext = rightEnumerator.MoveNext();
            if (leftMoveNext | rightMoveNext)
            {
               var leftValue = leftMoveNext ? leftEnumerator.Current : leftDefaultValue;
               var rightValue = rightMoveNext ? rightEnumerator.Current : rightDefaultValue;
               yield return (leftValue, rightValue);
            }
            else
            {
               break;
            }
         }
      }
   }

   public virtual IObject ZipL(ICollection collection, IObject leftDefaultValue, IObject rightDefaultValue, Lambda lambda)
   {
      return collectionClass.Revert(zipUnequal(List(), collection.GetIterator(false).List()), _typeConstraint);

      IEnumerable<IObject> zipUnequal(IEnumerable<IObject> left, IEnumerable<IObject> right)
      {
         using var leftEnumerator = left.GetEnumerator();
         using var rightEnumerator = right.GetEnumerator();
         while (true)
         {
            var leftMoveNext = leftEnumerator.MoveNext();
            var rightMoveNext = rightEnumerator.MoveNext();
            if (leftMoveNext | rightMoveNext)
            {
               var leftValue = leftMoveNext ? leftEnumerator.Current : leftDefaultValue;
               var rightValue = rightMoveNext ? rightEnumerator.Current : rightDefaultValue;
               yield return lambda.Invoke(leftValue, rightValue);
            }
            else
            {
               break;
            }
         }
      }
   }

   public IObject Unzip()
   {
      var list = collection.GetIterator(false).List();
      List<IObject> leftList = [];
      List<IObject> rightList = [];

      foreach (var obj in list)
      {
         if (obj is ICollection innerCollection)
         {
            var innerList = innerCollection.GetIterator(false).List();
            IObject[] twoList = [.. innerList.Take(2)];
            if (twoList.Length >= 2)
            {
               leftList.Add(twoList[0]);
               rightList.Add(twoList[1]);
            }
         }
      }

      var leftCollection = collectionClass.Revert(leftList, _typeConstraint);
      var rightCollection = collectionClass.Revert(rightList, _typeConstraint);
      List<IObject> newCollection = [leftCollection, rightCollection];

      return collectionClass.Revert(newCollection, _typeConstraint);
   }

   public IObject Unzip(Lambda lambda)
   {
      var list = collection.GetIterator(false).List();
      List<IObject> leftList = [];
      List<IObject> rightList = [];

      foreach (var obj in list)
      {
         var result = lambda.Invoke(obj);
         if (result is ICollection resultCollection)
         {
            var resultList = resultCollection.GetIterator(false).List().ToList();
            if (resultList.Count >= 2)
            {
               leftList.Add(resultList[0]);
               rightList.Add(resultList[1]);
            }
         }
         else
         {
            throw incompatibleClasses(result, "Collection");
         }
      }

      var leftCollection = collectionClass.Revert(leftList, _typeConstraint);
      var rightCollection = collectionClass.Revert(rightList, _typeConstraint);
      List<IObject> newCollection = [leftCollection, rightCollection];

      return collectionClass.Revert(newCollection, _typeConstraint);
   }

   public IObject Min()
   {
      var result = Unassigned.Value;
      foreach (var value in List())
      {
         if (result is Unassigned)
         {
            switch (value)
            {
               case IObjectCompare:
                  result = value;
                  break;
               default:
                  return Unassigned.Value;
            }
         }
         else if (value is IObjectCompare oc && oc.Compare(result) < 0)
         {
            result = value;
         }
      }

      return result;
   }

   public IObject Min(Lambda lambda)
   {
      var result = Unassigned.Value;
      if (lambda.Invokable.Parameters.Length == 2)
      {
         foreach (var value in List())
         {
            if (result is Unassigned || ((Int)lambda.Invoke(value, result)).Value < 0)
            {
               result = value;
            }
         }
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
            {
               throw incompatibleClasses(valueResult, "Object compare");
            }
         }
      }

      return result;
   }

   public IObject Max()
   {
      var result = Unassigned.Value;
      foreach (var value in List())
      {
         if (result is Unassigned)
         {
            switch (value)
            {
               case IObjectCompare:
                  result = value;
                  break;
               default:
                  return Unassigned.Value;
            }
         }
         else if (value is IObjectCompare oc && oc.Compare(result) > 0)
         {
            result = value;
         }
      }

      return result;
   }

   public IObject Max(Lambda lambda)
   {
      var result = Unassigned.Value;
      if (lambda.Invokable.Parameters.Length == 2)
      {
         foreach (var value in List())
         {
            if (result is Unassigned || ((Int)lambda.Invoke(value, result)).Value < 0)
            {
               result = value;
            }
         }
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
            {
               throw incompatibleClasses(valueResult, "Object compare");
            }
         }
      }

      return result;
   }

   public virtual IObject First() => List().FirstOrNone().Map(Some.Object) | (() => KNil.NilValue);

   public virtual IObject First(Lambda predicate)
   {
      foreach (var value in List().Where(value => predicate.Invoke(value).IsTrue))
      {
         return new Some(value);
      }

      return KNil.NilValue;
   }

   public IObject Last()
   {
      var reversed = List().Reverse();
      return reversed.FirstOrNone().Map(Some.Object) | (() => KNil.NilValue);
   }

   public IObject Last(Lambda predicate)
   {
      var reversed = List().Reverse();
      foreach (var value in reversed.Where(value => predicate.Invoke(value).IsTrue))
      {
         return new Some(value);
      }

      return KNil.NilValue;
   }

   public IObject Split(Lambda predicate)
   {
      List<IObject> ifTrue = [];
      List<IObject> ifFalse = [];
      foreach (var value in List())
      {
         if (predicate.Invoke(value).IsTrue)
         {
            ifTrue.Add(value);
         }
         else
         {
            ifFalse.Add(value);
         }
      }

      return collectionClass.Revert(
         new List<IObject> { collectionClass.Revert(ifTrue, _typeConstraint), collectionClass.Revert(ifFalse, _typeConstraint) }, _typeConstraint);
   }

   public IObject Split(int count)
   {
      List<IObject> ifTrue = [];
      List<IObject> ifFalse = [];
      var i = 0;
      foreach (var value in List())
      {
         if (i++ < count)
         {
            ifTrue.Add(value);
         }
         else
         {
            ifFalse.Add(value);
         }
      }

      return collectionClass.Revert(
         new List<IObject> { collectionClass.Revert(ifTrue, _typeConstraint), collectionClass.Revert(ifFalse, _typeConstraint) }, _typeConstraint);
   }

   public virtual IObject GroupBy(Lambda lambda)
   {
      var memo = new Memo<IObject, List<IObject>>.Function(_ => []);
      foreach (var item in List())
      {
         var key = lambda.Invoke(item);
         memo[key].Add(item);
      }

      var result = new Hash<IObject, IObject>();

      foreach (var key in memo.GetHash().KeyArray())
      {
         result[key] = collectionClass.Revert(memo[key], _typeConstraint);
      }

      return new Dictionary(result);
   }

   public IObject GroupBy(Lambda keyLambda, Lambda valueLambda)
   {
      var memo = new Memo<IObject, List<IObject>>.Function(_ => []);
      foreach (var item in List())
      {
         var key = keyLambda.Invoke(item);
         var value = valueLambda.Invoke(item);
         memo[key].Add(value);
      }

      var result = new Hash<IObject, IObject>();

      foreach (var key in memo.GetHash().KeyArray())
      {
         result[key] = collectionClass.Revert(memo[key], _typeConstraint);
      }

      return new Dictionary(result);
   }

   public KBoolean One(Lambda predicate)
   {
      var one = false;
      foreach (var _ in List().Where(value => predicate.Invoke(value).IsTrue))
      {
         if (one)
         {
            return new KBoolean(false);
         }
         else
         {
            one = true;
         }
      }

      return new KBoolean(true);
   }

   public KBoolean None(Lambda predicate)
   {
      return List().All(value => !predicate.Invoke(value).IsTrue);
   }

   public KBoolean Any(Lambda predicate) => List().Any(value => predicate.Invoke(value).IsTrue);

   public KBoolean All(Lambda predicate) => List().All(value => predicate.Invoke(value).IsTrue);

   public INumeric Sum()
   {
      var (_head, tail) = List().HeadAndTail();
      if (_head is (true, var head and INumeric))
      {
         var sum = (INumeric)head;
         foreach (var value in tail)
         {
            if (value is INumeric numeric)
            {
               sum = (INumeric)apply(sum, numeric, (x, y) => x + y, (x, y) => x + y, (x, y) => x + y, (x, y) => x.Add(y), "+");
            }
            else
            {
               throw incompatibleClasses(value, "Numeric");
            }
         }

         return sum;
      }
      else
      {
         return zero<Int>();
      }
   }

   public IObject CumulativeSum()
   {
      List<IObject> cumulativeSums = [];
      var (_head, tail) = List().HeadAndTail();
      if (_head is (true, var head and INumeric))
      {
         var sum = (INumeric)head;
         foreach (var value in tail)
         {
            if (value is INumeric numeric)
            {
               sum = (INumeric)apply(sum, numeric, (x, y) => x + y, (x, y) => x + y, (x, y) => x + y, (x, y) => x.Add(y), "+");
               cumulativeSums.Add((IObject)sum);
            }
            else
            {
               throw incompatibleClasses(value, "Numeric");
            }
         }

         return collectionClass.Revert(cumulativeSums, _typeConstraint);
      }
      else
      {
         return collectionClass.Revert([], _typeConstraint);
      }
   }

   public INumeric Average()
   {
      var sum = Sum();
      var length = collection.GetIterator(false).List().Count(value => value is INumeric);

      return (INumeric)apply(sum, (Int)length, (x, y) => x / y, (x, y) => x / y, (x, y) => x / y, (x, y) => x.Divide(y), "/");
   }

   public INumeric Product()
   {
      var (_head, tail) = List().HeadAndTail();
      if (_head is (true, var head and INumeric))
      {
         var product = (INumeric)head;
         foreach (var value in tail)
         {
            if (value is INumeric numeric)
            {
               product = (INumeric)apply(product, numeric, (x, y) => x * y, (x, y) => x * y, (x, y) => x * y, (x, y) => x.Multiply(y), "*");
            }
            else
            {
               throw incompatibleClasses(value, "Numeric");
            }
         }

         return product;
      }
      else
      {
         return one<Int>();
      }
   }

   public IObject CumulativeProduct()
   {
      List<IObject> cumulativeProducts = [];
      var (_head, tail) = List().HeadAndTail();
      if (_head is (true, var head and INumeric))
      {
         var product = (INumeric)head;
         foreach (var value in tail)
         {
            if (value is INumeric numeric)
            {
               product = (INumeric)apply(product, numeric, (x, y) => x * y, (x, y) => x * y, (x, y) => x * y, (x, y) => x.Multiply(y), "*");
               cumulativeProducts.Add((IObject)product);
            }
            else
            {
               throw incompatibleClasses(value, "Numeric");
            }
         }

         return collectionClass.Revert(cumulativeProducts, _typeConstraint);
      }
      else
      {
         return collectionClass.Revert([], _typeConstraint);
      }
   }

   public IObject Cross(ICollection collection)
   {
      List<List<IObject>> result = [];
      foreach (var left in List())
      {
         result.AddRange(collection.GetIterator(false).List().Select(right => new List<IObject> { left, right }));
      }

      return collectionClass.Revert(result.Select(l => collectionClass.Revert(l, _typeConstraint)), _typeConstraint);
   }

   public IObject Cross(ICollection collection, Lambda lambda)
   {
      var result = new List<IObject>();
      foreach (var left in List())
      {
         result.AddRange(collection.GetIterator(false).List().Select(right => lambda.Invoke(left, right)));
      }

      return collectionClass.Revert(result, _typeConstraint);
   }

   public virtual IObject By(int count)
   {
      switch (count)
      {
         case <= 0:
            return Flatten();
         case > 1:
         {
            List<IObject> outer = [];
            List<IObject> inner = [];
            foreach (var value in List())
            {
               inner.Add(value);
               if (inner.Count == count)
               {
                  outer.Add(collectionClass.Revert(inner, _typeConstraint));
                  inner.Clear();
               }
            }

            if (inner.Count > 0)
            {
               outer.Add(collectionClass.Revert(inner, _typeConstraint));
            }

            return collectionClass.Revert(outer, _typeConstraint);
         }
         default:
            return collectionClass.Revert(List(), _typeConstraint);
      }
   }

   public IObject Window(int count)
   {
      if (count > 1)
      {
         var list = List().ToList();
         if (list.Count >= count)
         {
            var lastIndex = list.Count - 1;
            List<IObject> outerList = [];
            var escape = false;
            for (var i = 0; i < list.Count && !escape; i++)
            {
               List<IObject> innerList = [];
               for (var j = i; j < i + count; j++)
               {
                  innerList.Add(list[j]);
                  if (j == lastIndex)
                  {
                     escape = true;
                  }
               }

               var result = collectionClass.Revert(innerList, _typeConstraint);
               outerList.Add(result);
            }

            return collectionClass.Revert(outerList, _typeConstraint);
         }
         else
         {
            return collectionClass.Revert(list, _typeConstraint);
         }
      }
      else
      {
         return collectionClass.Revert(List(), _typeConstraint);
      }
   }

   public IObject Shape(int rows, int columns)
   {
      List<IObject> source = [.. flatten(this)];
      List<IObject> outerList = [];
      var i = 0;

      if (rows > 0)
      {
         for (var row = 0; row < rows; row++)
         {
            List<IObject> innerList = [];
            for (var column = 0; column < columns; column++)
            {
               if (i >= source.Count)
               {
                  i = 0;
               }

               innerList.Add(source[i++]);
            }

            var innerCollection = collectionClass.Revert(innerList, _typeConstraint);
            outerList.Add(innerCollection);
         }
      }
      else
      {
         for (var column = 0; column < columns; column++)
         {
            if (i >= source.Count)
            {
               i = 0;
            }

            outerList.Add(source[i++]);
         }
      }

      return collectionClass.Revert(outerList, _typeConstraint);
   }

   public virtual IObject Unique() => collectionClass.Revert(List().Distinct(), _typeConstraint);

   public virtual IObject Unique(Lambda lambda)
   {
      return collectionClass.Revert(unique(List()), _typeConstraint);

      IEnumerable<IObject> unique(IEnumerable<IObject> list)
      {
         List<IObject> result = [];
         foreach (var obj in list)
         {
            if (!result.AtLeastOne(i => lambda.Invoke(i, obj).IsTrue))
            {
               result.Add(obj);
            }
         }

         foreach (var obj in result)
         {
            yield return obj;
         }
      }
   }

   public IObject Span(Lambda predicate)
   {
      var whileTrue = true;
      List<IObject> isTrue = [];
      List<IObject> isFalse = [];

      foreach (var value in List())
      {
         switch (whileTrue)
         {
            case true when predicate.Invoke(value).IsTrue:
               isTrue.Add(value);
               break;
            case true:
               whileTrue = false;
               isFalse.Add(value);
               break;
            default:
               isFalse.Add(value);
               break;
         }
      }

      return collectionClass.Revert(
         new List<IObject> { collectionClass.Revert(isTrue, _typeConstraint), collectionClass.Revert(isFalse, _typeConstraint) }, _typeConstraint);
   }

   public IObject Span(int count)
   {
      List<IObject> isTrue = [];
      List<IObject> isFalse = [];

      foreach (var value in List())
      {
         if (isTrue.Count < count)
         {
            isTrue.Add(value);
         }
         else
         {
            isFalse.Add(value);
         }
      }

      return collectionClass.Revert(
         new List<IObject> { collectionClass.Revert(isTrue, _typeConstraint), collectionClass.Revert(isFalse, _typeConstraint) }, _typeConstraint);
   }

   public IObject Shuffle()
   {
      IObject[] array = [.. List()];
      return shuffle(array, array.Length);
   }

   public IObject Random()
   {
      IObject[] array = [.. List()];
      var random = new Random(NowServer.Now.Millisecond);
      var i = random.Next(array.Length);

      return array[i];
   }

   public IObject Collect() => collectionClass.Revert(List(), _typeConstraint);

   public KArray ToArray() => new(List());

   public List ToList() => Objects.List.NewList(List());

   public KTuple ToTuple() => new([.. List()]);

   public Dictionary ToDictionary(Lambda keyLambda, Lambda valueLambda)
   {
      Hash<IObject, IObject> hash = [];

      foreach (var item in List())
      {
         var key = keyLambda.Invoke(item);
         var value = valueLambda.Invoke(item);
         hash[key] = value;
      }

      return new Dictionary(hash);
   }

   public IObject ToDictionary() => new Dictionary(List());

   public IObject ToSet() => new Set((IObject[])[.. List()]);

   public virtual IObject Each(Lambda action)
   {
      foreach (var item in List())
      {
         if (item is KTuple tuple)
         {
            action.Invoke(tuple.Value);
         }
         else
         {
            action.Invoke(item);
         }
      }

      return this;
   }

   public IObject Rotate(int count)
   {
      List<IObject> list = [.. List()];

      if (count > 0)
      {
         var rotatedList = list.Take(count);
         List<IObject> retainedList = [..list.Skip(count)];
         retainedList.AddRange(rotatedList);
         list = retainedList;
      }
      else
      {
         var length = list.Count;
         List<IObject> rotatedList = [.. list.Skip(length + count)];
         var retainedList = list.Take(length + count);
         rotatedList.AddRange(retainedList);
         list = rotatedList;
      }

      return collectionClass.Revert(list, _typeConstraint);
   }

   public IObject Shift(int count)
   {
      List<IObject> list = [.. List()];
      if (count == 0)
      {
         return collectionClass.Revert(list, _typeConstraint);
      }

      var defaultValue = classOf(list[0]).DefaultValue;

      if (count > 0)
      {
         List<IObject> retainedList = [.. list.Skip(count)];
         for (var i = 0; i < count; i++)
         {
            retainedList.Add(defaultValue);
         }

         list = retainedList;
      }
      else
      {
         var length = list.Count;
         List<IObject> rotatedList = [.. list.Skip(length + count)];
         for (var i = 0; i < length + count; i++)
         {
            rotatedList.Add(defaultValue);
         }

         list = rotatedList;
      }

      return collectionClass.Revert(list, _typeConstraint);
   }

   public IObject Shift(int count, IObject defaultValue)
   {
      List<IObject> list = [.. List()];

      if (count > 0)
      {
         List<IObject> retainedList = [..list.Skip(count)];
         for (var i = 0; i < count; i++)
         {
            retainedList.Add(defaultValue);
         }

         list = retainedList;
      }
      else
      {
         var length = list.Count;
         List<IObject> rotatedList = [.. list.Skip(length + count)];
         for (var i = 0; i < length + count; i++)
         {
            rotatedList.Add(defaultValue);
         }

         list = rotatedList;
      }

      return collectionClass.Revert(list, _typeConstraint);
   }

   protected static void rotateRight(List<IObject> list, int count)
   {
      var temp = list[count - 1];
      list.RemoveAt(count - 1);
      list.Insert(0, temp);
   }

   public IObject Permutations(int count)
   {
      return collectionClass.Revert(permutations(List(), count).Select(l => collectionClass.Revert(l, _typeConstraint)), _typeConstraint);

      IEnumerable<List<IObject>> permutations(IEnumerable<IObject> items, int length)
      {
         if (length == 1)
         {
            foreach (var item in items)
            {
               yield return [item];
            }
         }
         else
         {
            List<IObject> list = [.. items];
            foreach (var item in list)
            {
               List<IObject> remainingItems = [..list];
               remainingItems.Remove(item);
               foreach (var permutation in permutations(remainingItems, length - 1))
               {
                  yield return [item, ..permutation];
               }
            }
         }
      }
   }

   public IObject Permutations() => Permutations(collection.Length.Value);

   public IObject Combinations(int count)
   {
      return collectionClass.Revert(combinations(List(), count).Select(l => collectionClass.Revert(l, _typeConstraint)), _typeConstraint);

      IEnumerable<List<IObject>> combinations(IEnumerable<IObject> items, int length)
      {
         if (length == 0)
         {
            yield return [];
         }
         else
         {
            var itemList = items.ToList();
            for (var i = 0; i < itemList.Count; i++)
            {
               var currentItem = itemList[i];
               var remainingItems = itemList.Skip(i + 1);
               foreach (var combination in combinations(remainingItems, length - 1))
               {
                  yield return [currentItem, ..combination];
               }
            }
         }
      }
   }

   public IObject Combinations() => Combinations(collection.Length.Value);

   protected static IEnumerable<IObject> flatten(IIterator iterator)
   {
      var className = ((IObject)iterator.Collection).ClassName;

      while (iterator.Next() is (true, var item))
      {
         if (item.ClassName == className)
         {
            var innerIterator = ((ICollection)item).GetIterator(false);
            foreach (var inner in flatten(innerIterator))
            {
               yield return inner;
            }
         }
         else
         {
            yield return item;
         }
      }
   }

   protected static IEnumerable<IObject> flatten(IEnumerable<IObject> enumerable)
   {
      foreach (var item in enumerable)
      {
         if (item is ICollection collection)
         {
            var innerIterator = collection.GetIterator(false);
            foreach (var inner in flatten(innerIterator.List()))
            {
               yield return inner;
            }
         }
         else
         {
            yield return item;
         }
      }
   }

   public IObject Flatten() => collectionClass.Revert(flatten(this), _typeConstraint);

   public IObject Copy() => collectionClass.Revert(List(), _typeConstraint);

   public IObject Apply(ICollection collection)
   {
      List<Lambda> lambdas = [..List().Select(l => (Lambda)l)];
      List<IObject> list = [.. collection.GetIterator(false).List()];

      var result = applyAgainst(lambdas, list);
      return collectionClass.Revert(result, _typeConstraint);
   }

   public IObject Column(int column)
   {
      List<IObject> result = [];
      var columnIndex = Int.IntObject(column);
      while (true)
      {
         if (Next() is (true, var item))
         {
            if (classOf(item).RespondsTo("[](_)"))
            {
               var value = sendMessage(item, "[](_)", columnIndex);
               result.Add(value);
            }
            else
            {
               break;
            }
         }
         else
         {
            break;
         }
      }

      return collectionClass.Revert(result, _typeConstraint);
   }

   public IObject Partition(Lambda lambda)
   {
      List<IObject> matched = [];
      List<IObject> notMatched = [];

      foreach (var obj in List())
      {
         var result = lambda.Invoke(obj);
         if (result.IsTrue)
         {
            matched.Add(obj);
         }
         else
         {
            notMatched.Add(obj);
         }
      }

      return collectionClass.Revert([collectionClass.Revert(matched, _typeConstraint), collectionClass.Revert(notMatched, _typeConstraint)],
         _typeConstraint);
   }

   public IObject Pick(int count)
   {
      var random = new Random(NowServer.Now.Millisecond);
      List<IObject> result = [];
      Set<int> pickedIndexes = [];

      var list = collection.GetIterator(false).List().ToList();
      for (var i = 0; i < count; i++)
      {
         var randomIndex = random.Next(list.Count);
         while (pickedIndexes.Contains(randomIndex))
         {
            randomIndex = random.Next(list.Count);
         }

         pickedIndexes.Add(randomIndex);
         result.Add(list[randomIndex]);

         if (pickedIndexes.Count == list.Count)
         {
            break;
         }
      }

      return collectionClass.Revert(result, _typeConstraint);
   }

   public IObject Pick()
   {
      var random = new Random(NowServer.Now.Millisecond);
      var list = collection.GetIterator(false).List().ToList();

      return list[random.Next(list.Count)];
   }

   public IObject Roll(int count)
   {
      var random = new Random(NowServer.Now.Millisecond);
      List<IObject> result = [];
      IObject[] array = [.. List()];
      for (var i = 0; i < count; i++)
      {
         var randomIndex = random.Next(array.Length);
         result.Add(array[randomIndex]);
      }

      return collectionClass.Revert(result, _typeConstraint);
   }

   public IObject Splat(int count)
   {
      var list = List().ToList();
      if (count <= list.Count)
      {
         List<IObject> result = [];
         List<IObject> remainder = [];

         for (var i = 0; i < count; i++)
         {
            result.Add(list[i]);
         }

         for (var i = count; i < list.Count; i++)
         {
            remainder.Add(list[i]);
         }

         if (remainder.Count > 0)
         {
            result.Add(collectionClass.Revert(remainder, _typeConstraint));
         }

         return collectionClass.Revert(result, _typeConstraint);
      }
      else
      {
         throw fail("Requested count must be less than collection length");
      }
   }

   public virtual IObject Chunked(int count) => new ChunkedIterator(collection, count);

   public virtual IObject Windowed(int size, int step, bool partial) => new WindowedIterator(collection, size, step, partial);

   public IObject Repeated()
   {
      var memo = new Memo<IObject, int>.Function(_ => 0);
      foreach (var item in List())
      {
         memo[item]++;
      }

      Set<IObject> set = [];
      foreach (var (key, count) in memo)
      {
         if (count > 1)
         {
            set.Add(key);
         }
      }

      return new Set(set);
   }

   public IObject Accumulate(Lambda lambda)
   {
      var length = collection.Length.Value;
      return length switch
      {
         0 or 1 => (IObject)collection,
         _ => collectionClass.Revert(accumulate(lambda), _typeConstraint)
      };

      IEnumerable<IObject> accumulate(Lambda lambda)
      {
         List<IObject> list = [.. List()];
         var accum = list[0];
         yield return accum;

         foreach (var item in list.Skip(1))
         {
            var invoked = lambda.Invoke(accum, item);
            yield return invoked;

            accum = invoked;
         }
      }
   }

   public IObject Accumulate(IObject initialValue, Lambda lambda)
   {
      var length = collection.Length.Value;
      return length switch
      {
         0 or 1 => (IObject)collection,
         _ => collectionClass.Revert(accumulate(lambda), _typeConstraint)
      };

      IEnumerable<IObject> accumulate(Lambda lambda)
      {
         List<IObject> list = [.. List()];
         var accum = initialValue;
         yield return accum;

         foreach (var item in list)
         {
            var invoked = lambda.Invoke(accum, item);
            yield return invoked;

            accum = invoked;
         }
      }
   }

   public KBoolean AllTrue(IObject argument)
   {
      foreach (var predicate in List())
      {
         if (!pipeline(argument, predicate).IsTrue)
         {
            return false;
         }
      }

      return true;
   }

   public KBoolean AnyTrue(IObject argument)
   {
      foreach (var predicate in List())
      {
         if (pipeline(argument, predicate).IsTrue)
         {
            return true;
         }
      }

      return false;
   }

   public KBoolean NoneTrue(IObject argument)
   {
      foreach (var predicate in List())
      {
         if (pipeline(argument, predicate).IsTrue)
         {
            return false;
         }
      }

      return true;
   }

   public KTuple HeadTail()
   {
      var _next = Next();
      if (_next is (true, var next))
      {
         return new KTuple(new NameValue("head", Some.Object(next)), new NameValue("tail", this));
      }
      else
      {
         return new KTuple(new NameValue("head", KNil.NilValue), new NameValue("tail", this));
      }
   }

   public Junction JunctionAll() => new(JunctionType.All, List());

   public Junction JunctionAny() => new(JunctionType.Any, List());

   public Junction JunctionNone() => new(JunctionType.None, List());

   public Junction JunctionOne() => new(JunctionType.One, List());

   public IObject Step(int step)
   {
      if (step <= 1)
      {
         return collectionClass.Revert(List(), _typeConstraint);
      }

      var list = List().ToList();
      List<IObject> result = [];
      for (var i = 0; i < list.Count; i++)
      {
         if (i % step == 0)
         {
            result.Add(list[i]);
         }
      }

      return collectionClass.Revert(result, _typeConstraint);
   }

   public virtual IObject this[int index]
   {
      get
      {
         if (index > -1)
         {
            Maybe<IObject> _next = nil;
            for (var i = 0; i < index; i++)
            {
               _next = Next();
            }

            return _next.Required("Iterator index out of bounds");
         }
         else
         {
            var list = List().ToList();
            index = wrapIndex(index, list.Count);
            return list[index];
         }
      }
   }

   public Sequence Seq() => new(List());

   public IObject Transpose()
   {
      var list = List().ToList();
      if (list.Count == 0 || list[0] is not ICollection)
      {
         return (IObject)collection;
      }

      List<List<IObject>> transposed = [];
      foreach (var item in list)
      {
         if (item is ICollection innerCollection)
         {
            List<IObject> innerList = [.. innerCollection.GetIterator(false).List()];
            for (var i = 0; i < innerList.Count; i++)
            {
               if (transposed.Count <= i)
               {
                  transposed.Add([]);
               }

               transposed[i].Add(innerList[i]);
            }
         }
         else
         {
            return (IObject)collection;
         }
      }

      return collectionClass.Revert(transposed.Select(inner => collectionClass.Revert(inner, _typeConstraint)), _typeConstraint);
   }

   public IObject Assoc(IObject target)
   {
      foreach (var item in List())
      {
         if (item is ICollection innerCollection)
         {
            var first = innerCollection.GetIterator(false).First();
            if (first is Some some && some.Value.IsEqualTo(target))
            {
               return Some.Object(item);
            }
         }
      }

      return KNil.NilValue;
   }

   public IObject At(int index)
   {
      var returnValue = KNil.NilValue;
      for (var i = 0; i <= index; i++)
      {
         if (Next() is (true, var value))
         {
            returnValue = value;
         }
         else
         {
            return KNil.NilValue;
         }
      }

      return Some.Object(returnValue);
   }

   public IObject DotProduct(ICollection otherCollection)
   {
      var rightIterator = otherCollection.GetIterator(false);
      var result = Int.Zero;

      while (Next() is (true, var left) && left is INumeric && rightIterator.Next() is (true, var right) && right is INumeric)
      {
         var multiplied = Multiply.Apply(left, right);
         result = Add.Apply(result, multiplied);
      }

      return result;
   }

   protected static IEnumerable<IObject> applyAgainst(List<Lambda> lambdas, List<IObject> enumerable)
   {
      return lambdas.SelectMany(_ => enumerable, (lambda, item) => lambda.Invoke(item));
   }

   protected IObject shuffle(IObject[] array, int count)
   {
      var result = new Hash<int, IObject>();
      var random = new Random(NowServer.Now.Millisecond);
      for (var i = 0; i < count; i++)
      {
         var key = random.Next(array.Length);
         while (result.ContainsKey(key))
         {
            key = random.Next(array.Length);
         }

         result[key] = array[key];
      }

      return collectionClass.Revert(result.ValueArray(), _typeConstraint);
   }

   public TypeConstraint EquivalentTypeConstraint() => TypeConstraint.FromList("Collection");
}