using System.Collections.Generic;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using Standard.Types.Exceptions;
using Standard.Types.Monads;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Invokables
{
	public class YieldingInvokable : IInvokable, ICollection, IObject
	{
		Selector selector;
		List<IObject> cached;

		public YieldingInvokable(Selector selector, Parameters parameters, string image)
		{
			this.selector = selector;
			Parameters = parameters;
			Image = image;
			cached = new List<IObject>();
		}

		public Selector Selector => selector;

		public int Index { get; set; } = -1;

		public int Address { get; set; } = -1;

		public Parameters Parameters { get; }

		public string ClassName => "YieldingInvokable";

		public string AsString => selector.AsString;

		public string Image { get; }

		public bool Constructing => false;

		public int Hash => selector.Hash;

		public bool IsEqualTo(IObject obj) => obj is YieldingInvokable yfi && selector.IsEqualTo(yfi.selector);

		public bool Match(IObject comparisand, Hash<string, IObject> bindings) => false;

		public bool IsTrue => false;

		public Arguments Arguments { get; set; } = Arguments.Empty;

		public FrameGroup Frames { get; set; } = new FrameGroup();

		public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

		public IMaybe<IObject> Next(int index)
		{
			if (Machine.Current.Invoke(this).If(out var result))
				switch (result)
				{
					case Nil _:
						return none<IObject>();
					case YieldReturn yr:
						Address = yr.Address + 1;
						Frames = yr.Frames;
						return yr.ReturnValue.Some();
					default:
						throw incompatibleClasses(result, "YieldReturn");
				}
			else
				return none<IObject>();
		}

		public IMaybe<IObject> Peek(int index) => throw "Peek not supported".Throws();

		public Int Length => cached.Count;

		public IEnumerable<IObject> List
		{
			get
			{
				var iterator = GetIterator(false);
				cached.Clear();

				while (true)
				{
					var next = iterator.Next();
					if (next.If(out var value))
					{
						cached.Add(value);
						yield return value;
					}
					else
						yield break;
				}
			}
		}

		public bool ExpandForArray => true;

		public Boolean In(IObject item) => cached.Contains(item);

		public Boolean NotIn(IObject item) => !cached.Contains(item);

		public IObject Times(int count) => this;

		public IIterator GetIndexedIterator() => new IndexedIterator(this);
	}
}