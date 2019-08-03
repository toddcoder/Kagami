using System;
using Core.Collections;
using Core.Objects;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
	public class Mixin : IObject, IEquatable<Mixin>
	{
		protected string name;
		protected Equatable<Mixin> equatable;

		public Mixin(string name)
		{
			this.name = name;
			equatable = new Equatable<Mixin>(this, "name");
		}

		public string ClassName => name;

		public string AsString => name;

		public string Image => $"mixin {name}";

		public int Hash => equatable.GetHashCode();

		public bool IsEqualTo(IObject obj) => obj is Mixin mixin && name == mixin.name;

		public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

		public bool IsTrue => true;

		public bool Equals(Mixin other) => equatable.Equals(other);

		public override bool Equals(object obj) => equatable.Equals(obj);

		public override int GetHashCode() => equatable.GetHashCode();
	}
}