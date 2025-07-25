using Core.Collections;
using Core.Monads;

namespace Kagami.Library.Objects;

public class NumericOpenRangeCollection(NumericOpenRange openRange) : IObject, ICollection
{
   private IObject current = openRange.Seed;

   public string ClassName => openRange.ClassName;

   public string AsString => openRange.AsString;

   public string Image => openRange.Image;

   public int Hash => openRange.Hash;

   public bool IsEqualTo(IObject obj) => openRange.IsEqualTo(obj);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => openRange.Match(comparisand, bindings);

   public bool IsTrue => openRange.IsTrue;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject this[SkipTake skipTake] => openRange[skipTake];

   public IIterator GetIterator(bool lazy) => new LazyIterator(this);

   public Maybe<IObject> Next(int index)
   {
      var returnValue = current;
      current = ((INumeric)current).Increment((INumeric)openRange.Increment);

      return returnValue.Some();
   }

   public Maybe<IObject> Peek(int index) => current.Some();

   public Int Length => openRange.Length;

   public bool ExpandForArray => openRange.ExpandForArray;

   public KBoolean In(IObject item) => openRange.In(item);

   public KBoolean NotIn(IObject item) => openRange.NotIn(item);

   public IObject Times(int count) => openRange.Times(count);

   public KString MakeString(string connector) => openRange.MakeString(connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IObject One() => this;

   public IObject Copy() => new NumericOpenRangeCollection((NumericOpenRange)openRange.Copy());
}