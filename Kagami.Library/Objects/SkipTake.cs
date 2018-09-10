using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Objects
{
	public class SkipTake : IObject
	{
		IObject source;
		IMaybe<IObject> result;

		public SkipTake(IObject source)
		{
			this.source = source;
			result = none<IObject>();
		}

		public string ClassName => "SkipTake";

		public string AsString => $"{source.AsString}{result.FlatMap(o => o.AsString, () => "")}";

		public string Image => $"{source.Image}{result.FlatMap(o => o.Image, () => "")}";

		public int Hash
		{
			get
			{
				var hash = 17;
				hash = 37 * source.Hash;
				hash = 37 * result.FlatMap(o => o.Hash, () => 0);

				return hash;
			}
		}

		public bool IsEqualTo(IObject obj)
		{
			return obj is SkipTake st && source.IsEqualTo(st.source) && result.FlatMap(o => o, () => Nil.NilValue)
				.IsEqualTo(st.result.FlatMap(o => o, () => Nil.NilValue));
		}

		public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

		public bool IsTrue => result.IsSome;

		public SkipTake Literal(IObject literal)
		{
			if (result.If(out var r))
				result = sendMessage(r, "~()", literal).Some();
			else
				result = literal.Some();

			return this;
		}

		public SkipTake Skip(int count)
		{
			source = sendMessage(source, "skip()", new Int(count));
			return this;
		}

		public SkipTake Take(int count)
		{
			var taken = sendMessage(source, "take()", new Int(count));
			Literal(taken);
			Skip(count);

			return this;
		}

		public SkipTake TakeRest()
		{
			var count = ((Int)sendMessage(source, "length".get())).Value;
			return Take(count);
		}

		public IObject FullResult => result.FlatMap(r => r, () => source);
	}
}