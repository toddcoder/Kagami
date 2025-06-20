﻿using Core.Collections;
using Core.Monads;

namespace Kagami.Library.Objects
{
	public class OpenRangeCollection : ICollection, IObject
	{
		protected IObject current;
		protected Lambda lambda;
		protected OpenRange openRange;

		public OpenRangeCollection(OpenRange openRange)
		{
			current = openRange.Seed;
			lambda = openRange.Lambda;
			this.openRange = openRange;
		}

		public IIterator GetIterator(bool lazy) => new LazyIterator(this);

		public Maybe<IObject> Next(int index)
		{
			var result = current;
			current = lambda.Invoke(current);

			return result.Some();
		}

		public Maybe<IObject> Peek(int index) => current.Some();

		public Int Length => openRange.Length;

		public bool ExpandForArray => openRange.ExpandForArray;

		public KBoolean In(IObject item) => openRange.In(item);

		public KBoolean NotIn(IObject item) => openRange.NotIn(item);

		public IObject Times(int count) => openRange.Times(count);

		public KString MakeString(string connector) => openRange.MakeString(connector);

		public IIterator GetIndexedIterator() => new IndexedIterator(this);

		public string ClassName => openRange.ClassName;

		public string AsString => openRange.AsString;

		public string Image => openRange.Image;

		public int Hash => openRange.Hash;

		public bool IsEqualTo(IObject obj) => openRange.IsEqualTo(obj);

		public bool Match(IObject comparisand, Hash<string, IObject> bindings) => openRange.Match(comparisand, bindings);

		public bool IsTrue => openRange.IsTrue;

      public Guid Id { get; init; } = Guid.NewGuid();

      public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);
   }
}