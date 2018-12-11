using System;
using Standard.Types.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
	public struct Failure : IObject, IResult, IMonad
	{
		public static IObject Object(string message) => new Failure(message);

		public Failure(Error error) : this() => Error = error;

		public Failure(string message) : this(new Error(message)) { }

		public string ClassName => "Failure";

		public string AsString => $"f\"{Error.Message.AsString}\"";

		public string Image => AsString;

		public int Hash => Error.Hash;

		public bool IsEqualTo(IObject obj) => obj is Failure failure && Error.IsEqualTo(failure.Error);

		public bool Match(IObject comparisand, Hash<string, IObject> bindings)
		{
			return match(this, comparisand, (f1, f2) => f1.Error.Match(f2.Error, bindings), bindings);
		}

		public bool IsTrue => false;

		public IObject Value => throw new Exception("No value");

		public Error Error { get; }

		public bool IsSuccess => false;

		public bool IsFailure => true;

		public IObject Map(Lambda lambda) => this;

		public IObject FlatMap(Lambda ifSuccess, Lambda ifFailure) => ifFailure.Invoke(Error);

		public IObject Bind(Lambda map) => Map(map);

		public IObject Unit(IObject obj) => new Failure(Error);
	}
}