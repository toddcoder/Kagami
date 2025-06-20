﻿using Kagami.Library.Objects;
using Core.Collections;
using Core.Monads;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Packages;

public class XRandom : IObject, ICollection
{
   protected Random random;

   public XRandom(int seed) => random = new Random(seed);

   public XRandom() => random = new Random();

   public Float NextFloat() => random.NextDouble();

   public Int Next() => random.Next();

   public IIterator GetIterator(bool lazy) => lazy ? new Iterator(this) : new LazyIterator(this);

   Maybe<IObject> ICollection.Next(int index) => Int.IntObject(random.Next()).Some();

   public Maybe<IObject> Peek(int index) => Int.IntObject(random.Next()).Some();

   public Int Length => -1;

   public bool ExpandForArray => false;

   public KBoolean In(IObject item) => false;

   public KBoolean NotIn(IObject item) => true;

   public IObject Times(int count)
   {
      return new KTuple(Enumerable.Range(0, count).Select(_ => Int.IntObject(random.Next())).ToArray());
   }

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public Int Next(int max) => random.Next(max);

   public Int Next(int min, int max) => random.Next(min, max);

   public string ClassName => "Random";

   public string AsString => "Random";

   public string Image => "Random";

   public int Hash => random.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is XRandom;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => random.Next() != 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject this[SkipTake skipTake] => Objects.CollectionFunctions.skipTake(this, skipTake);
}