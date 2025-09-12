using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct KIndex : IObject, ICollection
{
   public static KIndex StartIndex(IIndexed indexed) => new(0, 0, indexed.Length);

   public static KIndex EndIndex(IIndexed indexed) => new(indexed.LastIndex, indexed.LastIndex, indexed.Length);

   public static KIndex FullIndex(IIndexed indexed) => new(0, indexed.LastIndex, indexed.Length);

   private readonly int start;
   private readonly int end;
   private readonly int length;
   private readonly int count;

   public KIndex(int start, int end, int length) : this()
   {
      this.start = start;
      this.end = end;
      this.length = length;
      count = end - start + 1;
   }

   public string ClassName => "Index";

   public string AsString => $"{start}::{end}";

   public string Image => AsString;

   public int Hash => HashCode.Combine(start, end, length);

   public bool IsEqualTo(IObject obj) => obj is KIndex index && start == index.start && end == index.end && length == index.length;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => start != 0 && end != 0 && length != 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject GetFromCollection(ICollection collection)
   {
      var iterator = collection.GetIterator(false);
      var stage1 = (ICollection)iterator.Skip(start);

      if (count == 1)
      {
         return ((ICollection)stage1.GetIterator(false).Take(1)).One();
      }
      else
      {
         iterator = stage1.GetIterator(false);
         return iterator.Take(count);
      }
   }

   public IObject SetToCollection(ICollection collection, IObject source)
   {
      if (collection is IMutableCollection mutableCollection)
      {
         var skip = start;
         var take = count;

         switch (source)
         {
            case Some some:
            {
               List<IObject> list = [some.Value];
               return mutableCollection.Assign(new SkipTake(skip, take), list);
            }
            case ICollection sourceCollection:
            {
               var enumerable = sourceCollection.GetIterator(false).List();
               return mutableCollection.Assign(new SkipTake(skip, take), enumerable);
            }
            default:
               throw fail("Source must be a collection");
         }
      }
      else
      {
         throw fail("Target must be a mutable collection");
      }
   }

   public Int Start => start;

   public Int End => end;

   public KRange Range() => new((Int)start, (Int)end, true);

   public IIterator GetIterator(bool lazy) => Range().GetIterator(lazy);

   public Maybe<IObject> Next(int index) => Range().Next(index);

   public Maybe<IObject> Peek(int index) => Range().Peek(index);

   public Int Length => length;

   public bool ExpandForArray => true;

   public KBoolean In(IObject item) => Range().In(item);

   public KBoolean NotIn(IObject item) => Range().NotIn(item);

   public IObject Times(int count) => Range().Times(count);

   public KString MakeString(string connector) => Range().MakeString(connector);

   public IIterator GetIndexedIterator() => Range().GetIndexedIterator();

   public IObject One() => Range().One();

   public IObject Copy() => new KIndex(start, end, length);

   IIterator ICollection.Following(IObject following) => new MultiIterator(this, following);

   public IObject this[SkipTake skipTake] => Range()[skipTake];

   public KIndex StartSucc() => count == 1 ? Shift(1) : new KIndex(start + 1, end, length).Normalize();

   public KIndex StartPred() => count == 1 ? Shift(-1) : new KIndex(start - 1, end, length).Normalize();

   public KIndex EndSucc() => count == 1 ? Shift(1) : new KIndex(start, end + 1, length).Normalize();

   public KIndex EndPred() => count == 1 ? Shift(-1) : new KIndex(start, end - 1, length).Normalize();

   public KIndex Shift(int n) => new KIndex(start + n, end + n, length).Normalize();

   public KIndex Expand(int n) => new KIndex(start, end + n, length).Normalize();

   public KIndex Contract(int n) => new KIndex(start, end - n, length).Normalize();

   public KIndex Single() => new KIndex(start, start, length).Normalize();

   public KIndex Normalize() => new(start < 0 ? 0 : start, end >= length ? length - 1 : end < 0 ? 0 : end, length);
}